using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace VanPhongPham.Models
{
    public class TrainModelAI
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private static readonly object _lock = new object();


        public TrainModelAI()
        {
            _mlContext = new MLContext();
        }
        public void TrainModel(List<(string UserId, string ProductId, int? ViewCount, int? AddToCartCount, int? PurchaseCount)> rawData, bool appendData = false)
        {
            lock (_lock)
            {
                // Điểm tương tác
                const int viewScore = 1;
                const int addToCartScore = 3;
                const int purchaseScore = 5;

                var userMapping = rawData.Select(x => x.UserId).Distinct()
                                         .Select((user, index) => new { user, index })
                                         .ToDictionary(x => x.user, x => x.index);

                var productMapping = rawData.Select(x => x.ProductId).Distinct()
                                            .Select((prod, index) => new { prod, index })
                                            .ToDictionary(x => x.prod, x => x.index);

                // Chuyển đổi dữ liệu
                var trainingData = rawData.Select(x => new PurchaseData
                {
                    UserId = (uint)userMapping[x.UserId],
                    ProductId = (uint)productMapping[x.ProductId],
                    Label = (float)(x.ViewCount * viewScore + x.AddToCartCount * addToCartScore + x.PurchaseCount * purchaseScore)
                }).ToList();

                // Nếu muốn thêm dữ liệu, kết hợp dữ liệu cũ với dữ liệu mới
                if (appendData && _model != null)
                {
                    var currentModelPredictionEngine = _mlContext.Model.CreatePredictionEngine<PurchaseData, ProductPrediction>(_model);
                    var currentData = _mlContext.Data.LoadFromEnumerable(trainingData); // load dữ liệu mới để huấn luyện thêm
                    var newModel = _mlContext.Recommendation().Trainers.MatrixFactorization(
                            labelColumnName: "Label",
                            matrixColumnIndexColumnName: "UserIdKey",
                            matrixRowIndexColumnName: "ProductIdKey").Fit(currentData);
                    _model = newModel;
                }
                else
                {
                    // Huấn luyện lại mô hình nếu không phải thêm dữ liệu
                    var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                                    inputColumnName: "UserId", outputColumnName: "UserIdKey")
                                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                                    inputColumnName: "ProductId", outputColumnName: "ProductIdKey"))
                                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                                    labelColumnName: "Label",
                                    matrixColumnIndexColumnName: "UserIdKey",
                                    matrixRowIndexColumnName: "ProductIdKey"));

                    var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);
                    _model = pipeline.Fit(dataView);
                }

                // Lưu mô hình
                _mlContext.Model.Save(_model, null, "D:\\model.zip");
            }
        }

        public void TrainModelIfNotExists(List<(string UserId, string ProductId, int? ViewCount, int? AddToCartCount, int? PurchaseCount)> rawData)
        {
            var modelPath = "D:\\model.zip";

            // Nếu mô hình đã tồn tại, sẽ thêm dữ liệu mới vào mô hình
            if (System.IO.File.Exists(modelPath))
            {
                Console.WriteLine("Model already exists. Appending data...");
                TrainModel(rawData, appendData: true);
            }
            else
            {
                Console.WriteLine("Model does not exist. Training a new model...");
                TrainModel(rawData);
            }
        }


        public void LoadModel()
        {
            // Tải mô hình đã huấn luyện
            _model = _mlContext.Model.Load("D:\\model.zip", out var _);
        }

        public PredictionEngine<PurchaseData, ProductPrediction> GetPredictionEngine()
        {
            return _mlContext.Model.CreatePredictionEngine<PurchaseData, ProductPrediction>(_model);
        }
    }

}