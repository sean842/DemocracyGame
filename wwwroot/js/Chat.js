
const baseUrl = "https://localhost:7288/";
const hubUrl = baseUrl + "ChatHub";
// create connection.
const chathub = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .build();


chathub.start().catch(err => console.error(err));

//-------------------------------------------------------------------------------------

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

//-------------------------------------------------------------------------------------


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

//-------------------------------------------------------------------------------------


// call everyone to take the next law.
function callForNextLaw() {
    chathub.invoke("NextLaw");
}

// recive the call and call the next law.
chathub.on("GetNextLaw", () => {
    goToNextLaw();
});

