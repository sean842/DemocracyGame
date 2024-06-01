const lawsData = {
    "laws": [
        {
            "content": "First Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0
        },
        {
            "content": "Second Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0
        },
        {
            "content": "Third Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0
        },
        {
            "content": "Fourth Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0
        },
        {
            "content": "Fifth Law Content",
            "isImg": false,
            "imgContent": "",
            "for": 0,
            "avoids": 0,
            "against": 0
        }
    ]
};


let currentLawIndex = 0; // Variable to track the current law index


// Function to display the current law
function displayCurrentLaw() {
    const currentLaw = lawsData.laws[currentLawIndex];
    const lawContentElement = document.getElementById('law');
    lawContentElement.textContent = currentLaw.content;
    // Start the countdown timer when the page is loaded
//    startCountdown();
}

displayCurrentLaw();





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



// Function to handle button click event to go to the next law
function goToNextLaw() {
    currentLawIndex++;
    if (currentLawIndex >= lawsData.laws.length) {
        // Handle reaching the end of the laws array
        return
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





