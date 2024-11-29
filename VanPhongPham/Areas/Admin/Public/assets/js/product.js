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

async function saveProduct(productId = null) {
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

    // Nếu có productId, thêm vào FormData
    if (productId) {
        formData.append("productId", productId); // Để biết đây là update
    }

    // Xử lý hình ảnh chính
    if (productImageFile) {
        try {
            mainImageUrl = await uploadToCloudinary(productImageFile, 'product_imgs');
        } catch (error) {
            console.error("Chi tiết lỗi:", error);
            alert('Có lỗi xảy ra khi tải lên hình ảnh chính: ' + error.message);
            return;
        }
    } else {
        const imagePreview = document.getElementById('imagePreview');
        mainImageUrl = imagePreview.src; // Lưu trữ URL của ảnh cũ
    }
    formData.append("mainImageUrl", mainImageUrl);

    // Xử lý hình ảnh phụ
    const additionalImages = document.getElementById('additionalImages').files;
    const additionalImageUrls = [];

    const additionalImagesPreview = document.getElementById('additionalImagesPreview');

    // Lấy tất cả các thẻ img trong additionalImagesPreview
    const images = additionalImagesPreview.getElementsByTagName('img');

    // Duyệt qua tất cả các thẻ img để lấy src
    for (let i = 0; i < images.length; i++) {
        const file = images[i];
        if (file.src.startsWith('https:')) {
            additionalImageUrls.push(file.src);
        }
    }

    if (additionalImages.length > 0) {
        for (let i = 0; i < selectedFiles.length; i++) {
            const file = selectedFiles[i];
            try {
                const url = await uploadToCloudinary(file, 'product_imgs');
                if (!additionalImageUrls.includes(url)) {
                    additionalImageUrls.push(url);
                }
            } catch (error) {
                console.error("Chi tiết lỗi:", error);
                alert('Có lỗi xảy ra khi tải lên hình ảnh phụ: ' + error.message);
                return;
            }
        }
    }

    // Thêm danh sách URL hình ảnh phụ vào FormData
    formData.append("additionalImageUrlsJson", JSON.stringify(additionalImageUrls));
    try {
        const response = await fetch(productId ? '/Product/UpdateProduct' : '/Product/AddProduct', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();
        if (result.success) {
            alert(`Sản phẩm đã ${productId ? 'được cập nhật' : 'thêm mới'} thành công!`);
            window.location.href = '/Admin/Product/Index';
        } else {
            alert(result.message);
        }
    } catch (error) {
        alert('Lỗi khi gửi yêu cầu đến máy chủ.');
        console.error(error);
    }
}

document.getElementById('ProductForm').onsubmit = function (event) {
    if (!validateForm()) {
        event.preventDefault();
        return;
    }
    event.preventDefault();
    // Đồng bộ nội dung CKEditor với textarea gốc
    editorInstance.updateSourceElement(); // Cập nhật nội dung CKEditor
    // Gọi hàm để xử lý gửi dữ liệu
    const productId = document.getElementById('productId') ? document.getElementById('productId').value : null; // Lấy productId từ form (nếu có)
    saveProduct(productId); // Gọi hàm với productId
};

function validateForm() {
    var imageInput = document.getElementById("productImage").files.length;
    var imagePreview = document.getElementById('imagePreview').src;
    var previewHash = imagePreview[imagePreview.length-1];

    // Kiểm tra nếu không có ảnh chọn và không có ảnh preview
    if (imageInput === 0 && (imagePreview === "" || previewHash === "#" || imagePreview === undefined)) {
        document.getElementById("imageWarning").style.display = "block"; // Hiển thị cảnh báo nếu chưa chọn ảnh
        return false;  // Ngừng gửi form nếu không có ảnh
    } else {
        document.getElementById("imageWarning").style.display = "none";  // Ẩn cảnh báo nếu đã chọn ảnh
        return true;
    }
}

const selectedFiles = [];

// Xử lý sự kiện cho nút chọn ảnh sản phẩm
document.getElementById("customProductImageButton").addEventListener("click", function () {
    document.getElementById("productImage").click();
});

document.getElementById("productImage").addEventListener("change", function (event) {
    const file = event.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.getElementById("imagePreview");
            preview.src = e.target.result;
            preview.style.display = "block"; // Hiển thị ảnh xem trước
        };
        reader.readAsDataURL(file);
    } else {
        alert("Vui lòng chọn một ảnh sản phẩm!");
        return; // Dừng lại nếu không có file nào được chọn
    }
});

// Xử lý sự kiện cho nút chọn các hình ảnh liên quan
document.getElementById("customAdditionalImagesButton").addEventListener("click", function () {
    document.getElementById("additionalImages").click();
});

// Tạo nút để thêm từng hình ảnh
document.getElementById("additionalImages").addEventListener("change", function (event) {
    const files = event.target.files;
    const previewContainer = document.getElementById("additionalImagesPreview");

    Array.from(files).forEach(file => {
        selectedFiles.push(file); // Thêm ảnh vào mảng selectedFiles
        displayPreview(file, selectedFiles.length - 1, previewContainer);
    });
});

// Hiển thị ảnh xem trước
function displayPreview(file, index, container) {
    const reader = new FileReader();
    reader.onload = function (e) {
        const imageContainer = document.createElement("div");
        imageContainer.style.position = "relative";
        imageContainer.style.display = "inline-block";
        imageContainer.style.maxWidth = "25%";
        imageContainer.style.marginTop = "10px";

        const img = document.createElement("img");
        img.src = e.target.result;
        img.alt = "Hình ảnh liên quan";
        img.style.width = "100%";

        const removeButton = document.createElement("button");
        removeButton.innerHTML = "Xóa";
        removeButton.style.position = "absolute";
        removeButton.style.top = "5px";
        removeButton.style.right = "5px";
        removeButton.style.backgroundColor = "red";
        removeButton.style.color = "white";
        removeButton.style.border = "none";
        removeButton.style.borderRadius = "5px";
        removeButton.style.cursor = "pointer";
        removeButton.addEventListener("click", function () {
            selectedFiles.splice(index, 1); // Xóa ảnh khỏi mảng
            container.removeChild(imageContainer); // Xóa ảnh khỏi giao diện
        });

        imageContainer.appendChild(img);
        imageContainer.appendChild(removeButton);
        container.appendChild(imageContainer);
    };
    reader.readAsDataURL(file);
}
