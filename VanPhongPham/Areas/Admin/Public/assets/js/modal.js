// modal.js
let actionUrl = '';

function showModal(title, body, url, actionText) {
    $('#genericModalLabel').text(title); // Thay đổi tiêu đề
    $('#genericModalBody').text(body); // Thay đổi nội dung
    actionUrl = url; // Lưu URL để sử dụng khi xác nhận

    // Thay đổi văn bản nút xác nhận
    $('#confirmBtn').text(actionText).off('click').on('click', function () {
        window.location.href = actionUrl; // Điều hướng đến URL xóa
    });

    // Hiển thị modal
    $('#genericModal').modal('show');
}

// Hàm để xác nhận xóa
function confirmDelete(url) {
    showModal('Xác Nhận Xóa', 'Bạn có chắc chắn muốn xóa không? Hành động này không thể hoàn tác!', url, 'Xóa');
}

// Hàm để thông báo thành công
function showSuccessMessage(message) {
    showModal('Thành Công', message, '', 'Đóng');
}

// Mở modal với chức năng thêm
function openCategoryModal(id = null, name = '') {
    $('#categoryName').val('');                // Xóa tên cũ
    $('#categoryModalLabel').text("Thêm Danh Mục"); // Tiêu đề thêm mới
    $('#categoryModal').modal('show');             // Hiển thị modal
}

// Hàm lưu danh mục (thêm mới)
function saveCategory() {
    var categoryName = $('#categoryName').val(); // tên danh mục
    var url = '/Product/AddCategoryAJAX'; // URL để thêm danh mục mới

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            categoryName: categoryName // Gửi tên danh mục
        },
        success: function (response) {
            if (response.success) {
                alert("Danh mục đã được thêm mới!");
                $('#categoryModal').modal('hide');

                // Cập nhật UI để thêm danh mục mới vào select
                var newOption = `<option value="${response.newCategoryId}" selected>${categoryName}</option>`;
                $('select[name="category_id"]').append(newOption);

                // Thiết lập giá trị selected cho danh mục mới
                $('select[name="category_id"]').val(response.newCategoryId);

                // Thêm danh mục mới vào danh sách nếu cần
                $('#categoryList').append(`
                    <li id="category-${response.newCategoryId}">
                        ${categoryName}
                        <button onclick="openCategoryModal()" class="btn btn-sm btn-primary">Sửa</button>
                    </li>
                `);
            } else {
                alert("Thêm thất bại: " + response.message);
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu danh mục!");
        }
    });
}


