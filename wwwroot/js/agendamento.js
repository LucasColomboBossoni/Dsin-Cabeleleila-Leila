window.onload = function () {
    const token = localStorage.getItem("authToken");

    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = "http://localhost:5195/Usuario"; // REDIRECIONA PARA A PAGINA DO USUARIO
    }

    const form = document.getElementById('agendamento-form');
    form.addEventListener('submit', function (e) {
        e.preventDefault(); // EVITA O ENVIO PADRAO DO FORMULARIO

        const service = document.getElementById('service').value;
        const date = document.getElementById('date').value;

        // ENVIA UM PEDIDO PARA VERIFICAR SE EXISTE AGENDAMENTO NA MESMA SEMANA
        fetch(`http://localhost:5195/agendamentos/verificar?data=${date}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.existeAgendamento) {
                    const userChoice = confirm("Você já tem um agendamento nesta semana escolhida. Deseja agendar para a data já agendada?");
                    if (userChoice) {
                        // ENVIA O AGENDAMENTO COM A DATA DO OUTRO AGENDAMENTO DA MESMA SEMANA
                        const dataExistente = data.dataExistente;
                        enviarAgendamento(service, dataExistente);
                    } else {
                        //REALIZA O AGENDAMENTO COM A DATA QUE ELE ESCOLHEU
                        enviarAgendamento(service, date);
                        alert("Agendamento realizado com sucesso na data escolhida!");
                    }
                } else {
                    // NAO HÁ AGENDAMENTO NA MESMA SEMANA, ENTAO ENVIA O AGENDAMENTO NORMALMENTE
                    enviarAgendamento(service, date);
                }
            })
            .catch(error => {
                console.error('Erro ao verificar agendamentos:', error);
            });
    });
};
// ENVIA O AGENDAMENTO PARA O BACK-END
function enviarAgendamento(service, date) {
    // OBJETO agendamento REPRESENTA OS DADOS DO AGENDAMENTO QUE SERÃO ENVIADOS PARA O BACK-END
    const agendamento = {
        Servico: service,
        DataHora: date
    };

    // RECUPERA O TOKEN DE AUTENTICAÇÃO DO LOCALSTORAGE, CASO O USUÁRIO ESTEJA LOGADO
    const token = localStorage.getItem("authToken");

    // ENVIA UMA REQUISIÇÃO POST PARA CRIAR OS AGENDAMENTOS
    fetch('http://localhost:5195/agendamentos', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json', 
            'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify(agendamento) // CONVERTE O OBJETO agendamento EM UMA STRING JSON PARA O ENVIO
    })
        .then(response => response.json()) // TRATA A RESPOSTA DA REQUISIÇÃO E CONVERTE EM JSON
        .then(data => {
            // SELECIONA O ELEMENTO HTML ONDE A MENSAGEM SERÁ EXIBIDA
            const messageDiv = document.getElementById("agendamento-message");

            if (data.message) { // VERIFICA SE O BACK-END RETORNOU UMA MENSAGEM
                messageDiv.innerText = data.message; // EXIBE A MENSAGEM DE SUCESSO OU INFORMAÇÃO
            } else {
                messageDiv.innerText = "Erro ao agendar.";
            }
        })
        .catch(error => {
            // TRATA POSSÍVEIS ERROS DURANTE A REQUISIÇÃO
            console.error('Erro ao enviar o agendamento:', error);
            document.getElementById("agendamento-message").innerText = "Erro ao enviar o agendamento."; // EXIBE UMA MENSAGEM DE ERRO NO FRONT-END.
        });
}

