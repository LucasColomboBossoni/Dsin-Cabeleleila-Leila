const form = document.getElementById('register-form');
form.addEventListener('submit', function (event) {
    event.preventDefault(); // Previne o comportamento padrão do formulário

    const usuario = {
        nome: document.getElementById('Nome').value,
        email: document.getElementById('Email').value,
        senha: document.getElementById('Senha').value
    };

    fetch('http://localhost:5195/Usuario/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(usuario) // Envia o objeto no formato JSON
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                return response.json().then(err => {
                    throw new Error(err.error || 'Erro desconhecido.');
                });
            }
        })
        .then(data => {
            alert(data.message); // Mostra mensagem de sucesso
        })
        .catch(error => {
            console.error('Erro:', error);
            alert(error.message); // Mostra mensagem de erro
        });
});

document.getElementById("login-form").addEventListener("submit", function (e) {
    e.preventDefault(); // Previne o comportamento padrão do formulário

    const email = document.getElementById("login-email").value;
    const senha = document.getElementById("login-senha").value;

    const usuario = {
        email: email,
        senha: senha
    };

    fetch("http://localhost:5195/usuario/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(usuario)
    })
    .then(response => response.json())
    .then(data => {
        if (data.message) {
            // Salva o token no localStorage (ou sessionStorage)
            localStorage.setItem("authToken", data.token);

            alert("Login realizado com sucesso!");
            window.location.href = "http://localhost:5195"; // Redireciona para a página home
        } else {
            alert("Credenciais inválidas.");
        }
    })
    .catch(error => {
        alert("Erro ao realizar login.");
    });
});
// Botão de Logout
document.getElementById("logout-button").addEventListener("click", function () {
    // Remove o token de autenticação
    localStorage.removeItem("authToken");

    // Oculta o botão de logout e exibe o link de login
    document.getElementById("logout-link").style.display = "none";
    document.getElementById("login-link").style.display = "block";

    // Redireciona para a página de login
    window.location.href = "/Usuario";
});

// Exibe ou oculta o botão de login/logout com base no estado de autenticação
window.addEventListener("load", function () {
    const token = localStorage.getItem("authToken");
    if (token) {
        // Usuário autenticado
        document.getElementById("login-link").style.display = "none";
        document.getElementById("logout-link").style.display = "block";
    } else {
        // Usuário não autenticado
        document.getElementById("login-link").style.display = "block";
        document.getElementById("logout-link").style.display = "none";
    }
});





