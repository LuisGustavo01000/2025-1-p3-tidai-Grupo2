function validateLogin() {
    const email = document.getElementById('email')?.value.trim();
    const password = document.getElementById('password')?.value;

    if (!email || !password) {
        alert("Por favor, preencha todos os campos.");
        return false;
    }

    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    if (!emailRegex.test(email)) {
        alert("Por favor, insira um email válido.");
        return false;
    }

    return true;
}

document.getElementById('login-form')?.addEventListener('submit', function(event) {
    event.preventDefault(); 

    if (validateLogin()) {

        window.location.href = "../Home/Home.html";
    }
});
