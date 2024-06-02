//const lawsData = {
//    "laws": [
//        {
//            "content": "First Law Content",
//            "isImg": false,
//            "imgContent": "",
//            "for": 0,
//            "avoids": 0,
//            "against": 0
//        },
//        {
//            "content": "Second Law Content",
//            "isImg": false,
//            "imgContent": "",
//            "for": 0,
//            "avoids": 0,
//            "against": 0
//        },
//        {
//            "content": "Third Law Content",
//            "isImg": false,
//            "imgContent": "",
//            "for": 0,
//            "avoids": 0,
//            "against": 0
//        },
//        {
//            "content": "Fourth Law Content",
//            "isImg": false,
//            "imgContent": "",
//            "for": 0,
//            "avoids": 0,
//            "against": 0
//        },
//        {
//            "content": "Fifth Law Content",
//            "isImg": false,
//            "imgContent": "",
//            "for": 0,
//            "avoids": 0,
//            "against": 0
//        }
//    ]
//};


//let currentLawIndex = 0; // Variable to track the current law index


//// Function to display the current law
//function displayCurrentLaw() {
//    const currentLaw = lawsData.laws[currentLawIndex];
//    const lawContentElement = document.getElementById('law');
//    lawContentElement.textContent = currentLaw.content;
//    // Start the countdown timer when the page is loaded
//    //    startCountdown();
//}

//displayCurrentLaw();



document.getElementById('RedCard').addEventListener('click', handleButtonClick);
document.getElementById('YellowCard').addEventListener('click', handleButtonClick);
document.getElementById('GreenCard').addEventListener('click', handleButtonClick);

// Function to handle button click event
function handleButtonClick(event) {
    const buttonId = event.target.id; // Get the id of the clicked button

    // Call the function to create the popup dynamically
    createConfirmationPopup(buttonId);
}


// Function to create the confirmation popup dynamically
function createConfirmationPopup(buttonId) {
    // Remove any existing modal with the same ID
    const existingModal = document.getElementById('confirmationModal');
    if (existingModal) {
        existingModal.remove();
    }

    // Create and show the Bootstrap modal dynamically
    const modalDiv = document.createElement('div');
    modalDiv.classList.add('modal', 'fade');
    modalDiv.setAttribute('id', 'confirmationModal');
    modalDiv.setAttribute('tabindex', '-1');
    modalDiv.setAttribute('role', 'dialog');
    modalDiv.setAttribute('aria-labelledby', 'confirmationModalLabel');
    modalDiv.setAttribute('aria-hidden', 'true');
    modalDiv.innerHTML = `
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmationModalLabel">Confirmation</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <p>Are you sure you want to select ${buttonId}?</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" data-dismiss="modal" onclick="handleConfirmation('${buttonId}')">Confirm</button>
                    </div>
                </div>
            </div>
        `;
    document.body.appendChild(modalDiv);
    $('#confirmationModal').modal('show');
}


// Function to handle confirmation of the selection
function handleConfirmation(buttonId) {
    // Call approveSelection with the selected buttonId
    alert(`בחרת ב:  ${buttonId} המתן לתוצאות.`);
    let selection;// the var we will send. represent the user selection.
    switch (buttonId) {
        case "GreenCard":
            selection = "for";
            break;
        case "RedCard":
            selection = "against";
            break;
        default:
            selection = "avoids";
    }

    // sent the selection to the lecture by calling a function from Chat.js.
    SendVote(selection);
}




// Function to create the pie chart
//function createPieChart(currentLawIndex) {
//    // Remove the existing canvas if it exists
//    const existingCanvas = document.getElementById('myPieChart');
//    if (existingCanvas) {
//        existingCanvas.parentNode.removeChild(existingCanvas);
//    }
//    // Get the canvas element
//    const canvasContainer = document.getElementById('canvasContainer');
//    const canvas = document.createElement('canvas');
//    canvas.id = 'myPieChart';
//    canvasContainer.appendChild(canvas);

//    // Get the 2d context
//    const ctx = canvas.getContext('2d');

//    // Extract data for the current law
//    const law = lawsData.laws[currentLawIndex];
//    const labels = ['For', 'Against', 'Avoids'];
//    const dataValues = [law.for, law.against, law.avoids];
//    const backgroundColors = ['green', 'red', 'yellow'];

//    // Define the data for the pie chart
//    const data = {
//        labels: labels, // Labels for the pie chart
//        datasets: [{
//            label: 'Votes',
//            data: dataValues,
//            backgroundColor: backgroundColors
//        }]
//    };

//    // Create the pie chart
//    const myPieChart = new Chart(ctx, {
//        type: 'pie',
//        data: data,
//        options: {
//            legend: {
//                position: 'right', // Position of the legend
//            },
//            plugins: {
//                // Add plugin to display labels on the pie chart
//                datalabels: {
//                    color: '#fff', // Color of the labels
//                    formatter: (value, ctx) => {
//                        return ctx.chart.data.labels[ctx.dataIndex] + ': ' + value; // Display label and value
//                    }
//                }
//            }
//        }
//    });
//}




// Function to handle button click event to go to the next law
//function goToNextLaw() {
//    currentLawIndex++;
//    if (currentLawIndex >= lawsData.laws.length) {
//        // Handle reaching the end of the laws array
//        return
//    }
//    displayCurrentLaw();
//    // Show LawContentContainer
//    document.getElementById('LawContentContainer').style.display = 'flex';

//    // Hide PieChartContainer
//    document.getElementById('PieChartContainer').style.display = 'none';
//    // Remove the existing pie chart if it exists
//    const canvas = document.getElementById('myPieChart');
//    if (canvas) {
//        canvas.remove();
//    }
//}




