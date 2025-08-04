function logout() {
    // If using localStorage
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");

    // Or just clear all of localStorage (if safe)
    localStorage.clear();

    // If using sessionStorage
    sessionStorage.clear();

    window.location.href = "/Account/Logout";

}

function login() {

    window.location.href = "/Account/Login";

}


function checkAuth() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7150/is-authenticated", true);
    xhr.withCredentials = true; // Send cookies

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                var container = document.getElementById("auth-buttons");

                if (!container) {
                    console.error("Container #auth-buttons not found in DOM.");
                    return;
                }

                container.innerHTML = "";

                if (data.authenticated) {
                    container.innerHTML = `
                    <li id="logout" class="nav-item">
                        <a class="btn btn-outline-danger"
                           href="#">
                           Logout (${data.user})
                        </a>
                    </li>`;
                    //https://localhost:7150/Identity/Account/Logout
                    var btn = document.getElementById("logout");

                    if (btn) {
                        btn.addEventListener("click", function (event) {
                            event.preventDefault();
                            logout();
                            //window.location.href = btn.querySelector('a')?.href || "https://localhost:7150/Identity/Account/Logout";
                        });
                    }
                } else {
                    container.innerHTML = `
                        <li id="login" class="nav-item">
                            <a class="btn btn-outline-primary ms-2"
                               href="https://localhost:7150/Identity/Account/Login?returnUrl=https://localhost:7053/signin-oidc">Login</a>
                        </li>
                        <li class="nav-item">
                            <a class="btn btn-outline-secondary ms-2"
                               href="https://localhost:7150/Identity/Account/Register?returnUrl=https://localhost:7053">Register</a>
                        </li>`;
                    var btn = document.getElementById("login");

                    if (btn) {
                        btn.addEventListener("click", function (event) {
                            event.preventDefault();
                            login();
                            
                        });
                    }
                }
            } else {
                console.error("Auth check failed: HTTP status " + xhr.status);
            }
        }
    };

    xhr.onerror = function () {
        console.error("Auth check failed: request error.");
    };

    xhr.send();
}

document.addEventListener("DOMContentLoaded", function () {
    checkAuth();
});
