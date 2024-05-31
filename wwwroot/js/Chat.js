
const baseUrl = "https://localhost:7288/";
const hubUrl = baseUrl + "ChatHub";
// create connection.
const chathub = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();


chathub.start().catch(err => console.error(err));


function SendVote(voteType) {
    console.log("i am working");
    chathub.invoke("SendVote", voteType);
}


chathub.on("ReceiveVote", (voteType) => {
    console.log(`recieved: ${voteType}`);
     //Ensure voteType is valid (for, avoids, or against)
        if (voteType !== 'for' && voteType !== 'avoids' && voteType !== 'against') {
            console.error('Invalid vote type');
            return;
        }
    lawsData.laws[currentLawIndex][voteType]++;
    console.log(lawsData);
});



// call everyone to crate the pie chart.
function callPieChart() {
    chathub.invoke("CreateTheChart");
}

// when we get the call, we create the pie chart.
chathub.on("CreateThePieChart", () => {
    // Hide LawContentContainer
    document.getElementById('LawContentContainer').style.display = 'none';

    // Show PieChartContainer
    document.getElementById('PieChartContainer').style.display = 'block';

    createPieChart(currentLawIndex);
});


//document.addEventListener("DOMContentLoaded", function (event) {
//    startHubConnection();
//});



// start the connection.
//function startHubConnection() {
//    chathub.start()
//        .then(() => {
//            handleReceivedMessages();
//            console.log(chathub);
//        })
//        .catch(error => {
//            console.error(error.toString());
//        });
//}

//// Take care when we receive message.
//function handleReceivedMessages() {
//    chathub.on("ReceiveMessage", (user, message) => {
//        const li = document.createElement("li");
//        li.innerText = `${user}: ${message} `;
//        document.getElementById("messageList").appendChild(li);
//    });
//}


//function sendMessage () {
//    const username = document.getElementById("userInput").value;
//    const message = document.getElementById("messageInput").value;

//    chathub.invoke("SendMessage", username, message);

//}


