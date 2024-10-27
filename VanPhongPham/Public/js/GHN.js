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
    await initAddressDropdowns();

    // Lấy địa chỉ hiện tại
    const currentCity = document.getElementById('city').value; // ID thành phố hiện tại
    if (currentCity) {
        const districts = await getDistricts(currentCity); // Sử dụng getDistricts
        const districtDropdown = document.getElementById('district');

        // Thêm các quận vào dropdown
        districts.forEach(district => {
            const option = document.createElement('option');
            option.value = district.DistrictID; // Sửa lại tên biến cho đúng
            option.textContent = district.DistrictName;
            districtDropdown.appendChild(option);
        });

        const currentDistrict = document.getElementById('district').value; // ID quận hiện tại
        if (currentDistrict) {
            const wards = await getWards(currentDistrict); // Sử dụng getWards
            const wardDropdown = document.getElementById('ward');

            // Thêm các phường vào dropdown
            wards.forEach(ward => {
                const option = document.createElement('option');
                option.value = ward.WardCode; // Sử dụng WardCode cho đúng
                option.textContent = ward.WardName;
                wardDropdown.appendChild(option);
            });
        }
    }
}
