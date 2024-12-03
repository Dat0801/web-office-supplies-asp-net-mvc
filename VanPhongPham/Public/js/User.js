const apiKey = "AIzaSyC7PsMUUTQancVCjMl0NnbDVDl0tpWdZDY";

function fetchUserData() {
    const token = localStorage.getItem('token');
    if (!token) {
        console.error('Không có token, không thể truy cập dữ liệu người dùng.');
        return;
    }

    fetch('/Account/GetUser', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token
        }
    })
        .then(response => {
            if (!response.ok) {
                console.error(`Lỗi ${response.status}: ${response.statusText}`);
                return response.json().then(data => {
                    console.log("Lỗi:", data.message);
                    logout(); // Gọi hàm logout nếu có lỗi
                });
            }
            return response.json(); // Chú ý: đây là hàm trả về một promise
        })
        .then(data => {
            if (data.success) {
                // Lưu thông tin người dùng vào sessionStorage
                const user = {
                    MaTaiKhoan: data.user_id,
                    TenTaiKhoan: data.username,
                    ImageProfile: data.avt_url,
                    CartID: data.cart_id
                };
                sessionStorage.setItem('user', JSON.stringify(user)); // Lưu đối tượng JSON

                // Hiển thị thông tin người dùng
                displayUserName(); // Đảm bảo hàm này sử dụng dữ liệu từ sessionStorage
            } else {
                console.log("Lỗi:", data.message);
                logout(); // Gọi hàm logout nếu không thành công
            }
        })
        .catch(error => {
            console.error('Có lỗi xảy ra:', error);
            logout(); // Gọi hàm logout nếu có lỗi
        });
}

function isOver16(dateString) {
    const today = new Date();
    const birthDate = new Date(dateString);
    const birthYear = birthDate.getFullYear();
    const currentYear = today.getFullYear();

    return (currentYear - birthYear) >= 16;
}

function displayUserName() {
    const userData = sessionStorage.getItem("user");
    const userInfoElement = document.getElementById("user-info");
    const profileImageElement = document.getElementById("profile-image");
    const loginItem = document.getElementById("login-item");
    const registerItem = document.getElementById("register-item");
    const logoutItem = document.getElementById("logout-item");
    const cartLink = document.getElementById("cart-link");
    const cartBadge = cartLink.querySelector('.badge'); // Lấy phần tử badge

    if (userData) {
        const user = JSON.parse(userData);

        // Hiển thị tên người dùng và liên kết đến trang cá nhân
        userInfoElement.innerHTML = `<a href="/Profile/Index?MaTaiKhoan=${encodeURIComponent(user.MaTaiKhoan)}" style="text-decoration: none;">${user.TenTaiKhoan}</a>`;
        userInfoElement.style.display = 'block';

        // Hiển thị ảnh đại diện nếu có, hoặc dùng ảnh mặc định, và đặt visibility thành visible
        profileImageElement.src = user.ImageProfile || '/Public/img/icons/icons8-male-user-100.png';

        // Hiển thị nút đăng xuất, ẩn đăng nhập và đăng ký
        logoutItem.style.display = 'block';
        loginItem.style.display = 'none';
        registerItem.style.display = 'none';
        document.getElementById('profile-section').style.display = 'flex';

        // Đặt liên kết giỏ hàng đến trang giỏ hàng
        cartLink.href = `/Cart/Index?cart_id=${user.CartID}`;

        // Gọi phương thức để lấy số lượng sản phẩm trong giỏ hàng
        loadCartQuantity(user.CartID, cartBadge);
    } else {
        // Ẩn các phần tử khi chưa đăng nhập
        userInfoElement.innerHTML = '';
        userInfoElement.style.display = 'none';

        // Hiển thị nút đăng nhập và đăng ký
        loginItem.style.display = 'block';
        registerItem.style.display = 'block';
        logoutItem.style.display = 'none';
        document.getElementById('profile-section').style.display = 'none';

        // Đặt liên kết giỏ hàng đến trang đăng nhập
        cartLink.href = "/Auth/Login";

        // Reset số lượng giỏ hàng
        cartBadge.innerText = '';

    }
}

// Hàm mới để tải số lượng sản phẩm trong giỏ hàng
function loadCartQuantity(cartId, cartBadge) {
    fetch('/Cart/GetCartQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ cart_id: cartId }),
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json(); // Chuyển đổi phản hồi thành JSON
        })
        .then(data => {
            cartBadge.innerText = data.quantity; // Cập nhật số lượng giỏ hàng
        })
        .catch(error => {
            console.error("Error loading cart quantity:", error);
            cartBadge.innerText = '0';
        });
}

function logout() {
    localStorage.removeItem('token');
    sessionStorage.removeItem('user');

    displayUserName();

    // Kiểm tra xem đã chuyển hướng hay chưa
    if (!sessionStorage.getItem('redirected')) {
        sessionStorage.setItem('redirected', 'true'); // Đánh dấu đã chuyển hướng
        window.location.replace('/Home/Index');
    }
}


async function signUp(email, password, tenkhachhang, tentaikhoan, gender, dateofbirth) {
    const apiUrl = `https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=${apiKey}`;
    const data = {
        email: email,
        password: password,
        returnSecureToken: true,
    };

    try {
        const response = await fetch(apiUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json", // Sửa dấu nháy
            },
            body: JSON.stringify(data),
        });

        const result = await response.json();
        console.log(result);

        if (result.error) {
            console.error("Lỗi đăng ký: ", result.error.message);
            document.getElementById('registrationError').innerText = "Email đã được đăng ký.";
            document.getElementById('registrationError').style.display = 'block';
            return;
        }

        document.getElementById('registrationError').style.display = 'none';

        await sendEmailVerification(result.idToken);

        await fetchUserDataAndSaveToSQL(result.idToken, tenkhachhang, tentaikhoan, gender, dateofbirth);
    } catch (error) {
        console.error("Lỗi đăng ký: ", error.message);
    }
}

async function sendEmailVerification(idToken) {
    const emailVerificationUrl = `https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=${apiKey}`;
    const verificationData = {
        requestType: "VERIFY_EMAIL",
        idToken: idToken,
    };

    try {
        const response = await fetch(emailVerificationUrl, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(verificationData),
        });

        const result = await response.json();

        if (result.error) {
            console.error("Lỗi xác thực email: ", result.error.message);
            throw new Error(result.error.message);
        }

        document.getElementById('emailVerificationMessage').innerText = "Một email xác thực đã được gửi. Vui lòng kiểm tra email của bạn.";
        document.getElementById('emailVerificationMessage').style.display = 'block';
    } catch (error) {
        console.error("Lỗi khi gửi email xác thực: ", error.message);
    }
}

async function fetchUserDataAndSaveToSQL(idToken, tenKhachHang, tentaikhoan, gender, dateOfBirth) {
    const userDataUrl = `https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=${apiKey}`;
    const requestData = { idToken: idToken };

    try {
        const response = await fetch(userDataUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestData),
        });

        const userData = await response.json();

        if (userData.error) {
            console.error("Lỗi lấy dữ liệu người dùng:", userData.error.message);
            throw new Error(userData.error.message);
        }

        const user = userData.users[0];

        const saveData = {
            user_id: user.localId,
            full_name: tenKhachHang,
            username: tentaikhoan,
            email: user.email,
            gender: gender,
            dob: dateOfBirth,
            avt_url: "",
        };

        await saveUserDataToSQL(saveData);
        await AddCartSection(saveData.user_id);
    } catch (error) {
        console.error("Có lỗi xảy ra khi lấy dữ liệu người dùng:", error.message);
    }
}

async function saveUserDataToSQL(data) {
    try {
        const response = await fetch('/Account/SaveUserData', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            console.error("Lỗi khi lưu dữ liệu:", errorText);
            return;
        }

        const result = await response.json();

        if (result.success) {
            console.log(result.message);
        } else {
            console.error("Có lỗi khi lưu dữ liệu:", result.message);
        }
    } catch (error) {
        console.error("Có lỗi xảy ra khi lưu vào SQL Server:", error);
    }
}


async function AddCartSection(user_id) {
    try {
        const response = await fetch('/Cart/AddCartSection', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ user_id: user_id}),
        });

        const result = await response.json();
        console.log(result);
        return result.success;
    } catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra tên đăng nhập:", error);
        return false;
    }
}

async function SaveUserAddressToSQL(data, addressid) {
    try {
        const response = await fetch(`/Address/SaveUpdateUserAddress?address_id=${addressid}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
        });

        console.log(response);

        if (!response.ok) {
            console.error("Lỗi khi lưu dữ liệu:", errorText);
            return;
        }

        const result = await response.json();

        if (result.success) {
            console.log(result.message);
        } else {
            console.error("Có lỗi khi lưu dữ liệu địa chỉ: ", result.message);
        }
    } catch (error) {
        console.log("Có lỗi xảy ra khi lưu địa chỉ vào SQL Server: ", error);
    }
}


async function SignWithPassword(email, password) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=${apiKey}`;
    const bodydata = {
        email: email,
        password: password,
        returnSecureToken: true,
    };

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bodydata),
        });

        const result = await response.json();
        console.log(result);

        const errorImage = document.createElement('img');
        errorImage.src = "/Public/img/icons/icons8-error-100.png";
        errorImage.width = 30;
        errorImage.height = 30;
        errorImage.alt = "Error";
        errorImage.style.verticalAlign = "middle";
        errorImage.style.marginRight = "12px";

        const notification = document.getElementById('notificationLogin');

        if (result.idToken) {
            notification.style.display = 'none';
            const checkemail = await CheckVerifiedEmail(result.idToken);

            if (checkemail) {
                document.getElementById('notificationLogin').style.display = 'none';

                localStorage.setItem('token', result.idToken);

                const token = localStorage.getItem('token');

                const check = await fetch('/Account/GetUser', {
                    method: "GET",
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                });

                const result2 = await check.json();

                if (result2.success && result2.message === "Người dùng không tìm thấy.") {
                    notification.innerHTML = '';

                    const messageContainer = document.createElement('div');
                    messageContainer.style.display = 'flex';
                    messageContainer.style.alignItems = 'center';


                    messageContainer.appendChild(errorImage); // Thêm hình ảnh
                    messageContainer.appendChild(document.createTextNode(" Email của bạn hoặc Mật khẩu không đúng, vui lòng thử lại")); // Thêm văn bản


                    notification.appendChild(messageContainer); // Thêm cả hai vào phần thông báo


                    notification.style.display = 'block';
                    return false;
                }
                else {
                    notification.style.display = 'none';
                    return true;
                }
            }
            else {
                console.error("Chưa xác thực email");

                notification.innerHTML = '';

                const messageContainer = document.createElement('div');
                messageContainer.style.display = 'flex';
                messageContainer.style.alignItems = 'center';


                messageContainer.appendChild(errorImage);
                messageContainer.appendChild(document.createTextNode(" Chưa xác thực email. Vui lòng xác thực email!"));


                notification.appendChild(messageContainer);


                notification.style.display = 'block';
            }
        }
        else {
            console.error("Có lỗi khi lấy dữ liệu signWithPass:", result.message);

            notification.innerHTML = '';

            const messageContainer = document.createElement('div');
            messageContainer.style.display = 'flex';
            messageContainer.style.alignItems = 'center';


            messageContainer.appendChild(errorImage);
            messageContainer.appendChild(document.createTextNode(" Tên tài khoản của bạn hoặc Mật khẩu không đúng, vui lòng thử lại"));


            notification.appendChild(messageContainer);


            notification.style.display = 'block';
        }
    }
    catch (error) {
        console.error("Có lỗi xảy ra khi đăng nhập bằng mật khẩu:", error);
    }
}

async function CheckVerifiedEmail(idToken) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=${apiKey}`;

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ idToken: idToken }),
        });

        const result = await response.json();

        if (result.users && result.users.length > 0) {
            // Kiểm tra trạng thái xác thực email
            if (result.users[0].emailVerified) {
                return true; // Email đã được xác thực
            } else {
                return false; // Email chưa được xác thực
            }
        }
        else {
            console.error("Có lỗi khi xác thực email:", result.message);
        }
    }
    catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra xác thực email: ", error);
    }
}

async function checkUsername(username) {
    try {
        const response = await fetch('/Account/CheckUsernameExists', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username: username }),
        });

        const result = await response.json();
        console.log(result);
        return result.exists;
    } catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra tên đăng nhập:", error);
        return false;
    }
}

async function sendResetPasswordEmail(email) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=${apiKey}`;

    const data = {
        requestType: 'PASSWORD_RESET',
        email: email
    };

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();

        if (response.ok) {
            return true;
        } else {
            console.log('Có lỗi xảy ra:', result.error);
        }
    } catch (error) {
        console.error("Có lỗi xảy ra khi gửi email:", error);
    }
}

async function checkEmailExists(email) {
    try {
        const response = await fetch('/Account/CheckEmailUserExists', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email: email }),
        });

        const result = await response.json();

        return result.exists;
    } catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra email reset password:", error);
        return false;
    }
}

async function changePassword(email, currentpassword, updatepassword) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=${apiKey}`;
    const bodydata = {
        email: email,
        password: currentpassword,
        returnSecureToken: true,
    };

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bodydata),
        });

        const result = await response.json();
        console.log(result);

        // Kiểm tra xem có lỗi từ kết quả không
        if (result.error) {
            return false; // Mật khẩu cũ không chính xác
        } else {
            const url2 = `https://identitytoolkit.googleapis.com/v1/accounts:update?key=${apiKey}`;
            const bodydata2 = {
                idToken: result.idToken,
                password: updatepassword,
                returnSecureToken: true,
            };

            const response2 = await fetch(url2, {
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(bodydata2),
            });

            const result2 = await response2.json();

            console.log(result2);

            if (!result2.error) {
                logout();
                return true; // Đổi mật khẩu thành công
            }
        }
    } catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra password:", error);
    }

    return false;
}

async function validatePassword(email, currentpassword) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=${apiKey}`;
    const bodydata = {
        email: email,
        password: currentpassword,
        returnSecureToken: true,
    };

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bodydata),
        });

        const result = await response.json();
        console.log(result);

        // Kiểm tra xem có lỗi từ kết quả không
        if (result.error) {
            return false; // Mật khẩu cũ không chính xác
        } else 
        {
            return true;
        }
    } catch (error) {
        console.error("Có lỗi xảy ra khi kiểm tra password:", error);
    }

    return false;
}

function isValidPhoneNumber(phone) {
    const trimmedPhone = phone;
    return trimmedPhone.length === 10 && !isNaN(trimmedPhone);
}

function checkNumberphone(phone) {
    // Head number phone Viettel 
    const viettel = ['086', '096', '097', '039', '038', '037', '036', '035', '034', '033', '032'];

    // Head number phone Vinaphone
    const vinaphone = ['091', '094', '088', '083', '084', '085', '081', '082'];

    // Head number phone Mobifone
    const mobifone = ['070', '079', '077', '076', '078', '089', '090', '093'];

    // Head number phone Vietnamobile
    const vietnamobile = ['092', '052', '056', '058'];

    // Head number phone Gmobile
    const gmobile = ['099', '059'];

    // Head number Itelecom
    const itelecom = ['087'];

    // Concatenate all arrays into one
    const allPhoneNumbers = [...viettel, ...vinaphone, ...mobifone, ...vietnamobile, ...gmobile, ...itelecom];

    const phonePrefix = phone.substring(0, 3);
    if (allPhoneNumbers.includes(phonePrefix))
    {
        return true;
    } else {
        // Invalid phone number
        return false;
    }
}

async function SaveSetDefault(addressid) {
    try {
        const response = await fetch('/Address/SaveSetDefault', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ address_id: addressid }), // Đảm bảo truyền dữ liệu đúng định dạng
        });

        if (!response.ok) {
            const errorText = await response.text(); // Lấy lỗi nếu có
            console.error("Lỗi khi lưu dữ liệu:", errorText);
            return;
        }

        const result = await response.json();

        if (result.success) {
            console.log(result.message);
            location.reload(); // Tải lại trang để cập nhật thông tin
        } else {
            console.error("Có lỗi khi thiết lập mặc định: ", result.message);
        }
    } catch (error) {
        console.log("Có lỗi xảy ra khi lưu thiết lập mặc định vào SQL Server: ", error);
    }
}

async function DeleteUserAddress(addressid) {
    try {
        const response = await fetch('/Address/DeleteUserAddress', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ address_id: addressid }), // Đảm bảo truyền dữ liệu đúng định dạng
        });

        if (!response.ok) {
            const errorText = await response.text(); // Lấy lỗi nếu có
            console.error("Lỗi khi lưu dữ liệu:", errorText);
            return;
        }

        const result = await response.json();

        if (result.success) {
            console.log(result.message);
            location.reload(); // Tải lại trang để cập nhật thông tin
        } else {
            console.error("Có lỗi khi xóa địa chỉ ", result.message);
        }
    } catch (error) {
        console.log("Có lỗi xảy ra khi xóa địa chỉ vào SQL Server: ", error);
    }
}

async function changeEmail(idToken, email) {
    const url = `https://identitytoolkit.googleapis.com/v1/accounts:update?key=${apiKey}`;
    const bodydata = {
        idToken: idToken,
        email: email,
        returnSecureToken: true
    };

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bodydata),
        });

        const result = await response.json();
        console.log(result);

        // Kiểm tra xem có lỗi từ kết quả không
        if (result.error) {
            return false;
        }
        else
        {
            return true;
        }
    }
    catch (error)
    {
        console.error("Có lỗi xảy ra khi đổi email:", error);
    }
}
