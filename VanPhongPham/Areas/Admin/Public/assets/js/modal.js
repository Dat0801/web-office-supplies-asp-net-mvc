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
    showModal('Xác Nhận Xóa', 'Bạn có chắc chắn muốn xóa không?', url, 'Xóa');
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
                if (response.message) {
                    var userConfirmed = confirm(response.message);
                    if (userConfirmed) {
                        restoreCategory(response.existCategoryId);
                    }
                } else {
                    alert("Thêm thất bại!");
                }
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu danh mục!");
        }
    });
}

// Hàm phục hồi danh mục
function restoreCategory(categoryId) {
    $.ajax({
        url: '/Product/RecycleCategoryAJAX',
        type: 'POST',
        data: {
            category_id: categoryId
        },
        success: function (response) {
            if (response.success) {
                alert('Danh mục đã được khôi phục thành công!');
                $('#categoryModal').modal('hide');

                var newOption = `<option value="${response.e_category_id}" selected>${response.e_category_name}</option>`;
                $('select[name="category_id"]').append(newOption);

                $('select[name="category_id"]').val(response.e_category_id);

            } else {
                alert('Không thể khôi phục danh mục.');
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi khôi phục danh mục!");
        }
    });
}

// Mở modal với chức năng thêm thuộc tính
function openAttributeModal(id = null, name = '') {
    $('#attributeName').val('');                // Xóa tên cũ
    $('#attributeModalLabel').text("Thêm Thuộc Tính"); // Tiêu đề thêm mới
    $('#attributeModal').modal('show');             // Hiển thị modal
}

var selectedAttributeValues = []; // Mảng lưu trữ các giá trị đã chọn

function updateAttributeValues() {
    // Lấy các ID thuộc tính đã chọn từ các checkbox
    var attributeIds = [];
    $('#attributeCheckboxes input[type="checkbox"]:checked').each(function () {
        attributeIds.push($(this).val());
    });

    // Lưu trữ các giá trị thuộc tính đã chọn trước đó
    var currentSelectedValues = [];
    $('#attributeValueCheckboxes input[type="checkbox"]:checked').each(function () {
        currentSelectedValues.push($(this).val());
    });

    // Cập nhật lại mảng selectedAttributeValues để giữ các giá trị đã chọn
    selectedAttributeValues = currentSelectedValues;

    // Xóa hết các checkbox giá trị thuộc tính hiện tại
    $('#attributeValueCheckboxes').empty();

    // Duyệt qua danh sách giá trị thuộc tính và thêm checkbox dựa trên thuộc tính đã chọn
    $.each(attributeValues, function (index, value) {
        // Nếu ID thuộc tính hiện tại có trong danh sách thuộc tính đã chọn
        if (attributeIds.includes(value.Attribute_id.toString())) {
            var checkboxHtml = '<div class="form-check">';
            checkboxHtml += '<input class="form-check-input" type="checkbox" name="attribute_value_id" value="' + value.Attribute_value_id + '" id="attributeValue_' + value.Attribute_value_id + '" data-attribute-id="' + value.Attribute_id + '">';
            checkboxHtml += '<label class="form-check-label" for="attributeValue_' + value.Attribute_value_id + '">' + value.Value + '</label>';
            checkboxHtml += '</div>';
            $('#attributeValueCheckboxes').append(checkboxHtml);
        }
    });

    // Đánh dấu lại các giá trị đã chọn trước đó
    selectedAttributeValues.forEach(function (selectedValue) {
        $('#attributeValueCheckboxes input[type="checkbox"][value="' + selectedValue + '"]').prop('checked', true);
    });
}

// Hàm mở modal thêm giá trị thuộc tính
function openAttributeValueModal(attributeId = null) {
    $('#attributeValue').val(''); // Xóa giá trị cũ
    if (attributeId) {
        $('#attribute_id').val(attributeId); // Cập nhật ID thuộc tính
    }
    $('#attributeValueModalLabel').text("Thêm Giá Trị Thuộc Tính"); // Tiêu đề thêm mới
    $('#attributeValueModal').modal('show'); // Hiển thị modal
}

// Hàm được gọi khi người dùng click vào "Thêm"
function openAttributeValueModalFromCheckbox() {
    // Lấy attribute_id từ các checkbox đã chọn
    var selectedCheckbox = $('#attributeCheckboxes input[type="checkbox"]:checked');

    if (selectedCheckbox.length > 0) {
        var attributeId = selectedCheckbox.last().val(); // Lấy attribute_id của checkbox đầu tiên
        openAttributeValueModal(attributeId); // Gọi modal với attribute_id
    } else {
        alert("Vui lòng chọn một thuộc tính trước khi thêm giá trị thuộc tính.");
    }
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

                // Cập nhật UI để thêm thuộc tính mới vào danh sách select
                var newOption = `<option value="${response.newAttributeId}" selected>${attributeName}</option>`;
                $('select[name="attribute_id"]').append(newOption);

                // Cập nhật UI để thêm thuộc tính mới vào danh sách checkbox
                var newCheckbox = `
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" name="attribute_id" value="${response.newAttributeId}" id="attribute_${response.newAttributeId}" onchange="updateAttributeValues()" checked>
                        <label class="form-check-label" for="attribute_${response.newAttributeId}">
                            ${attributeName}
                        </label>
                    </div>`;
                $('#attributeCheckboxes').append(newCheckbox);
            } else {
                if (response.message) {
                    var userConfirmed = confirm(response.message);
                    if (userConfirmed) {
                        restoreAttribute(response.existAttributeId);
                    }
                } else {
                    alert("Thêm thất bại!");
                }
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu thuộc tính!");
        }
    });
}

// Hàm phục hồi thuộc tính
function restoreAttribute(attributeId) {
    $.ajax({
        url: '/Product/RecycleAttributeAJAX',
        type: 'POST',
        data: {
            attribute_id: attributeId
        },
        success: function (response) {
            if (response.success) {
                alert('Thuộc tính đã được khôi phục thành công!');
                $('#attributeModal').modal('hide');
                window.location.reload();
            } else {
                alert('Không thể khôi phục thuộc tính.');
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi khôi phục thuộc tính!");
        }
    });
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

                // Cập nhật UI để thêm giá trị mới vào danh sách checkbox và đánh dấu là checked
                var newCheckbox = `
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" name="attribute_value_id" value="${response.newAttributeValueId}" id="attributeValue_${response.newAttributeValueId}" checked>
                        <label class="form-check-label" for="attributeValue_${response.newAttributeValueId}">
                            ${attributeValue}
                        </label>
                    </div>`;
                $('#attributeValueCheckboxes').append(newCheckbox);
            } else {
                if (response.message) {
                    var userConfirmed = confirm(response.message);
                    if (userConfirmed) {
                        restoreAttributeValue(response.existAttributeValueId);
                    }
                } else {
                    alert("Thêm thất bại!");
                }
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi lưu giá trị thuộc tính!");
        }
    });
}

// Hàm phục hồi giá trị thuộc tính
function restoreAttributeValue(attributeValueId) {
    $.ajax({
        url: '/Product/RecycleAttributeValueAJAX',
        type: 'POST',
        data: {
            attribute_value_id: attributeValueId
        },
        success: function (response) {
            if (response.success) {
                alert('Giá trị đã được khôi phục thành công!');
                $('#attributeValueModal').modal('hide');

                // Thêm giá trị mới vào danh sách attributeValues
                attributeValues.push({
                    Attribute_id: response.e_attribute_id,
                    Attribute_value_id: response.e_attribute_value_id,
                    Value: response.e_value
                });

                // Cập nhật UI để thêm giá trị mới vào danh sách checkbox và đánh dấu là checked
                var newCheckbox = `
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" name="attribute_value_id" value="${response.e_attribute_value_id}" id="attributeValue_${response.e_attribute_value_id}" checked>
                        <label class="form-check-label" for="attributeValue_${response.e_attribute_value_id}">
                            ${response.e_value}
                        </label>
                    </div>`;
                $('#attributeValueCheckboxes').append(newCheckbox);
            } else {
                alert('Không thể khôi phục thuộc tính.');
            }
        },
        error: function () {
            alert("Có lỗi xảy ra khi khôi phục thuộc tính!");
        }
    });
}

