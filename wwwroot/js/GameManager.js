document.addEventListener("DOMContentLoaded", (event) => {
    displayCurrentLaw();
});


const lawsData = {
    "laws": [
        {
            "content": "First Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0,
            "isPassed": false 
        },
        {
            "content": "Second Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0,
            "isPassed": false 
        },
        {
            "content": "Third Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0,
            "isPassed": false 
        },
        {
            "content": "Fourth Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0,
            "isPassed": false 
        },
        {
            "content": "Fifth Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0,
            "isPassed": false 
        }
    ]
};

let currentLawIndex = 0; // Variable to track the current law index




// Function to display the current law
function displayCurrentLaw() {
    const currentLaw = lawsData.laws[currentLawIndex];
    const lawContentElement = document.getElementById('law');
    lawContentElement.innerText = currentLaw.content;
}


// לבדוק למה זה לא עובד כשאני קורא לגרף עוגה
function calculateResult() {

    const law = lawsData.laws[currentLawIndex];// get the current law.
    // take the vote for & against and calculate.
    const totalFor = law.for;
    const totalAgainst = law.against;
    if (totalFor > totalAgainst) {
        lawsData.laws[currentLawIndex].isPassed = true;
        return;
    }
    else {
        lawsData.laws[currentLawIndex].isPassed = false;
        return;
    }

}



function createPieChart(currentLawIndex) {
    // Remove the existing canvas if it exists
    const existingCanvas = document.getElementById('myPieChart');
    if (existingCanvas) {
        existingCanvas.parentNode.removeChild(existingCanvas);
    }


    // Get the canvas element
    const canvasContainer = document.getElementById('canvasContainer');
    const canvas = document.createElement('canvas');
    canvas.id = 'myPieChart';
    canvasContainer.appendChild(canvas);

    // Get the 2d context
    const ctx = canvas.getContext('2d');

    // Extract data for the current law
    const law = lawsData.laws[currentLawIndex];
    const labels = ['For', 'Against', 'Avoids'];
    const dataValues = [law.for, law.against, law.avoids];
    const backgroundColors = ['green', 'red', 'yellow'];

    // call func to calculatethe results.
    calculateResult();

    // Define the data for the pie chart
    const data = {
        labels: labels, // Labels for the pie chart
        datasets: [{
            label: 'Votes',
            data: dataValues,
            backgroundColor: backgroundColors
        }]
    };

    // Create the pie chart
    const myPieChart = new Chart(ctx, {
        type: 'pie',
        data: data,
        options: {
            legend: {
                position: 'right', // Position of the legend
            },
            plugins: {
                // Add plugin to display labels on the pie chart
                datalabels: {
                    color: '#fff', // Color of the labels
                    formatter: (value, ctx) => {
                        return ctx.chart.data.labels[ctx.dataIndex] + ': ' + value; // Display label and value
                    }
                }
            }
        }
    });

}




function goToNextLaw() {
    currentLawIndex++;
    if (currentLawIndex >= lawsData.laws.length) {
        // Handle reaching the end of the laws array
        FinishVote();
        return;
    }

    displayCurrentLaw();
    // Show LawContentContainer
    document.getElementById('LawContentContainer').style.display = 'flex';

    // Hide PieChartContainer
    document.getElementById('PieChartContainer').style.display = 'none';
    // Remove the existing pie chart if it exists
    const canvas = document.getElementById('myPieChart');
    if (canvas) {
        canvas.remove();
    }
}


function FinishVote() {
    //hide the game.
    document.getElementById("VoteScreen").style.display = "none";
    //show the results
    document.getElementById("resultsContainer").style.display = "block";
    displayLawsTable();
}

function displayLawsTable() {
    const tableContainer = document.getElementById('resultsContainer');
    // cout the law's that passed and not passed.
    let passed = 0;
    let notPassed = 0;
    for (i = 0; i < lawsData.laws.length; i++) {
        if (lawsData.laws[i].isPassed == true) {
            passed++;
        } else {
            notPassed++;
        }
    }

    // make a table.
    let tableHTML = `
        <table>
        <tr> <th>עברו</th> <th>נפלו</th> </tr>
        <tr> <td>${passed}</td> <td>${notPassed}</td> </tr>
        </table>
        `;

    // insert to html.
    tableContainer.innerHTML = tableHTML;

}
