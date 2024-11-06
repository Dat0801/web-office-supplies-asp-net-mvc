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
               
            } else {
                alert("Thêm thất bại");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu danh mục!");
        }
    });
}

// Mở modal với chức năng thêm thuộc tính
function openAttributeModal(id = null, name = '') {
    $('#attributeName').val('');                // Xóa tên cũ
    $('#attributeModalLabel').text("Thêm Thuộc Tính"); // Tiêu đề thêm mới
    $('#attributeModal').modal('show');             // Hiển thị modal
}

// Hàm lưu thuộc tính (thêm mới)
function saveAttribute() {
    var attributeName = $('#attributeName').val(); // tên thuộc tính
    var url = '/Product/AddAttributeAJAX'; // URL để thêm thuộc tính mới

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            attributeName: attributeName // Gửi tên thuộc tính
        },
        success: function (response) {
            if (response.success) {
                alert("Thuộc tính đã được thêm mới!");
                $('#attributeModal').modal('hide');

                // Cập nhật UI để thêm thuộc tính mới vào danh sách nếu cần
                var newOption = `<option value="${response.newAttributeId}" selected>${attributeName}</option>`;
                $('select[name="attribute_id"]').append(newOption);
                
            } else {
                alert("Thêm thất bại");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu thuộc tính!");
        }
    });
}

function updateAttributeValues() {
    var attributeIds = $('#attributeSelect').val() || []; // Lấy giá trị được chọn hoặc mảng rỗng

    // Xóa tất cả tùy chọn hiện tại
    $('#attributeValueSelect').empty();

    // Lọc và thêm các giá trị thuộc tính tương ứng
    $.each(attributeValues, function (index, value) {
        // Kiểm tra xem attributeId hiện tại có trong danh sách đã chọn không
        if (attributeIds.includes(value.Attribute_id.toString())) {
            $('#attributeValueSelect').append('<option value="' + value.Attribute_value_id + '">' + value.Value + '</option>');
        }
    });

    // Giữ lại các giá trị thuộc tính đã chọn từ sản phẩm
    if (attributeValuesForProduct) {
        attributeValuesForProduct.forEach(function (selectedValue) {
            // Kiểm tra xem giá trị đã được thêm vào dropdown chưa
            if ($('#attributeValueSelect option[value="' + selectedValue.Attribute_value_id + '"]').length > 0) {
                $('#attributeValueSelect option[value="' + selectedValue.Attribute_value_id + '"]').prop('selected', true);
            }
        });
    }
}

updateAttributeValues();

// Mở modal với chức năng thêm giá trị thuộc tính
function openAttributeValueModal(attributeId = null) {
    $('#attributeValue').val(''); // Xóa giá trị cũ
    $('#attribute_id').val(attributeId);
    $('#attributeValueModalLabel').text("Thêm Giá Trị Thuộc Tính"); // Tiêu đề thêm mới
    $('#attributeValueModal').modal('show'); // Hiển thị modal
}

// Hàm lưu giá trị thuộc tính (thêm mới)
function saveAttributeValue() {
    var attributeId = $('#attribute_id').val(); // ID thuộc tính đã chọn
    var attributeValue = $('#attributeValue').val(); // Giá trị thuộc tính
    var url = '/Product/AddAttributeValueAJAX'; // URL để thêm giá trị thuộc tính mới

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            attribute_id: attributeId, // Gửi ID thuộc tính
            value: attributeValue // Gửi giá trị thuộc tính
        },
        success: function (response) {
            if (response.success) {
                alert("Giá trị thuộc tính đã được thêm mới!");
                $('#attributeValueModal').modal('hide');

                // Thêm giá trị mới vào danh sách attributeValues
                attributeValues.push({
                    Attribute_id: attributeId,
                    Attribute_value_id: response.newAttributeValueId,
                    Value: attributeValue
                });

                // Cập nhật UI để thêm giá trị mới vào danh sách nếu cần
                var newOption = `<option value="${response.newAttributeValueId}" selected>${attributeValue}</option>`;
                $('select[name="attribute_value_id"]').append(newOption);
            } else {
                alert("Thêm thất bại");
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu giá trị thuộc tính!");
        }
    });
}



