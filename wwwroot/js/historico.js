window.onload = function () {
    const token = localStorage.getItem("authToken");

    if (!token) {
        alert("Você precisa estar logado para acessar esta página.");
        window.location.href = "http://localhost:5195/Usuario"; // Redireciona para a página de login
    }

    // Faz a requisição para obter os agendamentos do usuário
    fetch('http://localhost:5195/api/Historico/historico', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Erro na requisição: ${response.statusText}`);
            }
            return response.json();
        })
        .then(agendamentos => {
            const agendamentosList = document.getElementById('agendamentos-list');
            agendamentosList.innerHTML = '';

            if (agendamentos.length > 0) {
                agendamentos.forEach(agendamento => {
                    const agendamentoItem = document.createElement('div');
                    agendamentoItem.classList.add('agendamento-item');
                    agendamentoItem.innerHTML = `
                        <p><strong>Serviço:</strong> ${agendamento.servico}</p>
                        <p><strong>Data e Hora:</strong> ${new Date(agendamento.dataHora).toLocaleString()}</p>
                        <button onclick="editAgendamento(${agendamento.id}, '${agendamento.servico}', '${agendamento.dataHora}')">Editar</button>
                    `;
                    agendamentosList.appendChild(agendamentoItem);
                });
            } else {
                agendamentosList.innerHTML = '<p>Não há agendamentos realizados.</p>';
            }
        })
        .catch(error => {
            console.error('Erro ao buscar agendamentos:', error);
            document.getElementById('agendamento-message').innerText = "Erro ao buscar agendamentos.";
        });
};

function editAgendamento(id, servico, dataHora) {
    // Verificar se falta 2 dias para o agendamento
    const agendamentoDate = new Date(dataHora);
    const currentDate = new Date();
    const diffTime = agendamentoDate - currentDate; // Diferença em milissegundos
    const diffDays = diffTime / (1000 * 3600 * 24); // Converter a diferença para dias

    // Se a diferença for de 2 dias ou menos, cancelar a edição
    if (diffDays <= 2) {
        alert("Não é possível editar o agendamento com menos de 2 dias de antecedência. Se necessario o reagendamento ligar para o Salao. Tel: 99999-9999");
        return; // Interrompe a execução da função e não permite a edição
    }

    // Preencher o formulário com os dados do agendamento
    const form = document.getElementById('edit-agendamento-form');
    form.querySelector('input[name="id"]').value = id;
    form.querySelector('select[name="servico"]').value = servico;
    form.querySelector('input[name="dataHora"]').value = new Date(dataHora).toISOString().slice(0, 16); // Formatar para input datetime-local

    // Mostrar o formulário de edição
    document.getElementById('edit-agendamento-container').style.display = 'block';
}


// Submissão do formulário de edição
document.getElementById('edit-agendamento-form').addEventListener('submit', function (event) {
    event.preventDefault(); // Impede o comportamento padrão do formulário

    const token = localStorage.getItem("authToken");
    if (!token) {
        alert("Você precisa estar logado para editar um agendamento.");
        window.location.href = "http://localhost:5195/Usuario"; // Redireciona para a página de login
    }

    const id = document.querySelector('input[name="id"]').value;
    const servico = document.querySelector('select[name="servico"]').value;
    const dataHora = document.querySelector('input[name="dataHora"]').value;

    const agendamentoAtualizado = {
        id: id,
        servico: servico,
        dataHora: dataHora
    };

    // Requisição PUT para atualizar o agendamento
    fetch(`http://localhost:5195/api/Historico/editar/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(agendamentoAtualizado)
    })
        .then(response => response.json())
        .then(data => {
            alert("Agendamento atualizado com sucesso.");
            window.location.reload(); // Atualiza a página para refletir as mudanças
        })
        .catch(error => {
            console.error("Erro ao editar agendamento:", error);
            alert("Erro ao editar agendamento.");
        });
});