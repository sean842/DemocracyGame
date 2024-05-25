
const baseUrl = "https://localhost:7288/";
const hubUrl = baseUrl + "ChatHub";
const chathub = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();


document.addEventListener("DOMContentLoaded", function (event) {
    getHub();
});

function getHub() {
    chathub.start()
        .then(() => {
            subscribeToHubEvents();
        })
        .catch(error => {
            console.error(error.toString());
        });
}

function subscribeToHubEvents() {
    chathub.on("ReceiveMessage", (user, message) => {
        const li = document.createElement("li");
        li.innerText = `${user}: ${message} `;
        document.getElementById("messageList").appendChild(li);
    });
}


function clickBTN() {
    const username = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    chathub.invoke("SendMessage", username, message);

}

