
const baseUrl = "https://localhost:7288/";
const hubUrl = baseUrl + "ChatHub";
// create connection.
const chathub = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();


document.addEventListener("DOMContentLoaded", function (event) {
    startHubConnection();
});

// start the connection.
function startHubConnection() {
    chathub.start()
        .then(() => {
            handleReceivedMessages();
            console.log(chathub);
        })
        .catch(error => {
            console.error(error.toString());
        });
}

// Take care when we receive message.
function handleReceivedMessages() {
    chathub.on("ReceiveMessage", (user, message) => {
        const li = document.createElement("li");
        li.innerText = `${user}: ${message} `;
        document.getElementById("messageList").appendChild(li);
    });
}


function sendMessage () {
    const username = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    chathub.invoke("SendMessage", username, message);

}


function SendVote(voteType) {
    console.log("i am working");
    chathub.invoke("SendVote", voteType);
}
