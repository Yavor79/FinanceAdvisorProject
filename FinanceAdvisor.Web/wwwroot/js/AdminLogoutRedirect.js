function logout() {
    // If using localStorage
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");

    // Or just clear all of localStorage (if safe)
    localStorage.clear();

    // If using sessionStorage
    sessionStorage.clear();

    window.location.href = "/AdminAccount/AdminLogout";

}

function login() {

    window.location.href = "/Account/Login";

}


function loadLogoutBtn() {
    var container = document.getElementById("auth-buttons");

    if (!container) {
        console.error("Container #auth-buttons not found in DOM.");
        return;
    }

    container.innerHTML = `
    <li id="logout" class="nav-item">
        <a class="btn btn-outline-danger"
            href="#">
            Logout
        </a>
    </li>`;
    
    var btn = document.getElementById("logout");

    if (btn) {
        btn.addEventListener("click", function (event) {
            event.preventDefault();
            logout();
            //window.location.href = btn.querySelector('a')?.href || "https://localhost:7150/Identity/Account/Logout";
        });
    }
               
};

document.addEventListener("DOMContentLoaded", function () {
    loadLogoutBtn();
});
