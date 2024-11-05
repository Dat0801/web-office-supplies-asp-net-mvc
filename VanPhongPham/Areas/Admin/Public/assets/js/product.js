let editorInstance; // Khai báo biến toàn cục để lưu trữ instance của CKEditor

// Khởi tạo CKEditor
ClassicEditor
    .create(document.querySelector('#productDescription'))
    .then(editor => {
        editorInstance = editor; // Lưu instance vào biến toàn cục
    })
    .catch(error => {
        console.error(error);
    });
async function saveProduct() {
    const formElement = document.getElementById('ProductForm'); // Lấy form theo ID
    if (!formElement) {
        alert('Form không tồn tại!');
        return;
    }

    const formData = new FormData(formElement);
    const productImageFile = document.getElementById('productImage').files[0];
    let mainImageUrl = '';

    // Lấy dữ liệu từ CKEditor
    const editorData = editorInstance.getData();
    formData.append("description", editorData); // Thêm mô tả vào FormData

    // Thêm trạng thái vào FormData
    formData.append("status", document.querySelector('select[name="status"]').value);
    console.log('Status:', formData.get('status'));
    console.log('Description:', formData.get('description')); // Kiểm tra giá trị mô tả

    // Xử lý hình ảnh chính
    if (productImageFile) {
        try {
            mainImageUrl = await uploadToCloudinary(productImageFile, 'product_imgs');
            formData.append("mainImageUrl", mainImageUrl);
        } catch (error) {
            console.error("Chi tiết lỗi:", error);
            alert('Có lỗi xảy ra khi tải lên hình ảnh chính: ' + error.message);
            return;
        }
    }

    // Xử lý hình ảnh phụ
    const additionalImages = document.getElementById('additionalImages').files;
    const additionalImageUrls = [];

    for (let i = 0; i < additionalImages.length; i++) {
        const file = additionalImages[i];
        try {
            const url = await uploadToCloudinary(file, 'product_imgs');
            additionalImageUrls.push(url);
        } catch (error) {
            console.error("Chi tiết lỗi:", error);
            alert('Có lỗi xảy ra khi tải lên hình ảnh phụ: ' + error.message);
            return;
        }
    }

    // Thêm danh sách URL hình ảnh phụ vào FormData
    formData.append("additionalImageUrlsJson", JSON.stringify(additionalImageUrls));

    try {
        const response = await fetch('/Product/AddProduct', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            alert('Sản phẩm đã được thêm thành công!');
            window.location.href = '/Admin/Product/Index';
        } else {
            alert('Có lỗi xảy ra khi thêm sản phẩm!');
        }
    } catch (error) {
        alert('Lỗi khi gửi yêu cầu đến máy chủ.');
        console.error(error);
    }
}

document.getElementById('ProductForm').onsubmit = function (event) {
    event.preventDefault();
    // Đồng bộ nội dung CKEditor với textarea gốc
    editorInstance.updateSourceElement(); // Cập nhật nội dung CKEditor
    // Gọi hàm để xử lý gửi dữ liệu
    saveProduct();
};
