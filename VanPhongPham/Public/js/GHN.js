const TokenAPI = "89b97758-911d-11ef-8e53-0a00184fe694"; // Token của bạn

// Hàm lấy danh sách tỉnh
async function getProvinces() {
    const response = await fetch('https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI
        }
    });

    if (response.ok) {
        const data = await response.json();
        return data.data; // Trả về danh sách tỉnh
    } else {
        console.error('Lỗi khi lấy danh sách tỉnh:', response.statusText);
    }
}

async function getDistricts(provinceId) {
    const response = await fetch('https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI
        },
        body: JSON.stringify({ province_id: Number(provinceId) })
    });

    if (response.ok) {
        const data = await response.json();
        return data.data; // Trả về danh sách quận
    } else {
        console.error('Lỗi khi lấy danh sách quận:', response.statusText);
    }
}

async function getWards(districtId) {
    const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=${districtId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI
        },
        body: JSON.stringify({ district_id: Number(districtId) }) // Gửi ID quận
    });

    if (response.ok) {
        const data = await response.json();
        return data.data; // Trả về danh sách phường
    } else {
        console.error('Lỗi khi lấy danh sách phường:', response.statusText);
    }
}

async function getService(to_district) {
    try {

        const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/available-services`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'Token': TokenAPI
            },
            body: JSON.stringify({
                shop_id: 195136,
                from_district: 1456,
                to_district: parseInt(to_district)  // Truyền to_district như một số nguyên
            })
        });

        if (!response.ok) {
            const errorDetails = await response.json();
            console.error("Lỗi khi tính toán dịch vụ vận chuyển:", errorDetails);
        } else {
            const result = await response.json();
            return result.data;
        }
    } catch (error) {
        console.error("Lỗi trong quá trình gửi yêu cầu:", error);
    }
}

async function getFee(serviceid, todistrict, toward, weight, items) {
    const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee`, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI,
            'ShopId': 195136
        },
        body: JSON.stringify({ service_type_id: serviceid, to_district_id: parseInt(todistrict), to_ward_code: toward, weight: parseInt(weight), items: items })
    });

    if (response.ok) {
        const result = await response.json();
        return result.data;
    }
    else {
        console.error("Lỗi khi tính")
    }
}

async function getShippingDetail(ordercode) {
    const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail`, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI
        },
        body: JSON.stringify({ order_code: ordercode })
    });

    if (response.ok) {
        const result = await response.json();
        return result.data;
    }
    else {
        console.error("Lỗi khi lấy lịch sử giao hàng")
    }
}

async function CancelOrder(ordercode) {
    const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/v2/switch-status/cancel`, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI,
            'ShopId': 195136
        },
        body: JSON.stringify({ order_codes: Array.isArray(ordercode) ? ordercode : [ordercode] })
    });

    if (response.ok) {
        const result = await response.json();
        return result.data;
    }
    else {
        console.error("Lỗi khi hủy đơn hàng")
    }
}

async function createOrder(payment_type_id, to_name, to_phone, to_address, to_ward_name, to_district_name, to_province_name, cod_amount, weight, service_type_id, items) {
    const response = await fetch(`https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create`, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
            'Token': TokenAPI,
            'ShopId': 195136
        },
        body: JSON.stringify({
            payment_type_id: parseInt(payment_type_id),
            required_note: "KHONGCHOXEMHANG",
            to_name: to_name,
            to_phone: to_phone,
            to_address: to_address,
            to_ward_name: to_ward_name,
            to_district_name: to_district_name,
            to_province_name: to_province_name,
            cod_amount: parseFloat(cod_amount),
            weight: parseInt(weight),
            length: 1,
            width: 1,
            height: 1,
            service_type_id: parseInt(service_type_id),
            items: items
        })
    });

    if (response.ok) {
        const result = await response.json();
        return result.data;
    }
    else {
        console.error("Lỗi tạo đơn hàng")
    }
}

async function initAddressDropdowns() {
    const provinces = await getProvinces();
    const citySelect = document.getElementById('city');
    const districtSelect = document.getElementById('district');
    const wardSelect = document.getElementById('ward');

    // Thêm tỉnh vào select
    provinces.forEach(province => {
        const option = document.createElement('option');
        option.value = province.ProvinceID; // ProvinceID là ID tỉnh
        option.textContent = province.ProvinceName; // ProvinceName là tên tỉnh
        citySelect.appendChild(option);
    });

    citySelect.addEventListener('change', async function () {
        const selectedProvinceId = this.value;
        districtSelect.innerHTML = '<option selected disabled>Chọn Quận/Huyện</option>'; // Reset quận
        wardSelect.innerHTML = '<option selected disabled>Chọn Phường/Xã</option>'; // Reset phường

        const districts = await getDistricts(selectedProvinceId);
        districts.forEach(district => {
            const option = document.createElement('option');
            option.value = district.DistrictID; // DistrictID là ID quận
            option.textContent = district.DistrictName; // DistrictName là tên quận
            districtSelect.appendChild(option);
        });
    });

    districtSelect.addEventListener('change', async function () {
        const selectedDistrictId = this.value;
        wardSelect.innerHTML = '<option selected disabled>Phường/Xã</option>'; // Reset phường

        const wards = await getWards(selectedDistrictId);
        wards.forEach(ward => {
            const option = document.createElement('option');
            option.value = ward.WardCode; // WardID là ID phường
            option.textContent = ward.WardName; // WardName là tên phường
            wardSelect.appendChild(option);
        });
    });
}

async function initUpdateAddressDropdowns() {
    const provinces = await getProvinces();
    const citySelect = document.getElementById('upcity');
    const districtSelect = document.getElementById('updistrict');
    const wardSelect = document.getElementById('upward');

    // Thêm tỉnh vào select
    provinces.forEach(province => {
        const option = document.createElement('option');
        option.value = province.ProvinceID; // ProvinceID là ID tỉnh
        option.textContent = province.ProvinceName; // ProvinceName là tên tỉnh
        citySelect.appendChild(option);
    });

    citySelect.addEventListener('change', async function () {
        const selectedProvinceId = this.value;
        districtSelect.innerHTML = '<option selected disabled>Chọn Quận/Huyện</option>'; // Reset quận
        wardSelect.innerHTML = '<option selected disabled>Chọn Phường/Xã</option>'; // Reset phường

        const districts = await getDistricts(selectedProvinceId);
        districts.forEach(district => {
            const option = document.createElement('option');
            option.value = district.DistrictID; // DistrictID là ID quận
            option.textContent = district.DistrictName; // DistrictName là tên quận
            districtSelect.appendChild(option);
        });
    });

    districtSelect.addEventListener('change', async function () {
        const selectedDistrictId = this.value;
        wardSelect.innerHTML = '<option selected disabled>Phường/Xã</option>'; // Reset phường

        const wards = await getWards(selectedDistrictId);
        wards.forEach(ward => {
            const option = document.createElement('option');
            option.value = ward.WardCode; // WardID là ID phường
            option.textContent = ward.WardName; // WardName là tên phường
            wardSelect.appendChild(option);
        });
    });
}

async function initInitAddressDropdowns() {
    const provinces = await getProvinces();
    const citySelect = document.getElementById('initcity');
    const districtSelect = document.getElementById('initdistrict');
    const wardSelect = document.getElementById('initward');

    // Thêm tỉnh vào select
    provinces.forEach(province => {
        const option = document.createElement('option');
        option.value = province.ProvinceID; // ProvinceID là ID tỉnh
        option.textContent = province.ProvinceName; // ProvinceName là tên tỉnh
        citySelect.appendChild(option);
    });

    citySelect.addEventListener('change', async function () {
        const selectedProvinceId = this.value;
        districtSelect.innerHTML = '<option selected disabled>Chọn Quận/Huyện</option>'; // Reset quận
        wardSelect.innerHTML = '<option selected disabled>Chọn Phường/Xã</option>'; // Reset phường

        const districts = await getDistricts(selectedProvinceId);
        districts.forEach(district => {
            const option = document.createElement('option');
            option.value = district.DistrictID; // DistrictID là ID quận
            option.textContent = district.DistrictName; // DistrictName là tên quận
            districtSelect.appendChild(option);
        });
    });

    districtSelect.addEventListener('change', async function () {
        const selectedDistrictId = this.value;
        wardSelect.innerHTML = '<option selected disabled>Phường/Xã</option>'; // Reset phường

        const wards = await getWards(selectedDistrictId);
        wards.forEach(ward => {
            const option = document.createElement('option');
            option.value = ward.WardCode; // WardID là ID phường
            option.textContent = ward.WardName; // WardName là tên phường
            wardSelect.appendChild(option);
        });
    });
}
