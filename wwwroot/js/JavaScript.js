// Variable to hold the countdown interval
var countdownInterval;

// Function to start the countdown timer
function startCountdown() {
    // Get countdown element
    var countdownElement = document.getElementById('countdown');

    // Set initial countdown value
    var countdownValue = 10;

    // Update countdown display every second
    countdownInterval = setInterval(function () {
        countdownValue--;
        countdownElement.textContent = countdownValue;

        // Check if time is up
        if (countdownValue === 0) {
            clearInterval(countdownInterval); // Stop the countdown
            showPopup(); // Call the function to show the popup
        }
    }, 1000);
}

// Function to stop the countdown timer
function stopCountdown() {
    clearInterval(countdownInterval);
}

// Function to show the Bootstrap modal popup
function showPopup() {
    // Create and show the Bootstrap modal dynamically
    var modalDiv = document.createElement('div');
    modalDiv.classList.add('modal', 'fade');
    modalDiv.setAttribute('id', 'popupModal');
    modalDiv.setAttribute('tabindex', '-1');
    modalDiv.setAttribute('role', 'dialog');
    modalDiv.setAttribute('aria-labelledby', 'popupModalLabel');
    modalDiv.setAttribute('aria-hidden', 'true');
    modalDiv.innerHTML = `
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="popupModalLabel">Time's up!</h5>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <p>Sorry, your time has run out.</p>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-primary" data-dismiss="modal">Close</button>
          </div>
        </div>
      </div>
    `;
    document.body.appendChild(modalDiv);
    $('#popupModal').modal('show');
}

// Start the countdown timer when the page is loaded
startCountdown();

// Function to handle button click event
function handleButtonClick(event) {
    var buttonId = event.target.id; // Get the id of the clicked button

    // Call the function to create the popup dynamically
    createConfirmationPopup(buttonId);
}

// Function to create the confirmation popup dynamically
function createConfirmationPopup(buttonId) {
    // Create and show the Bootstrap modal dynamically
    var modalDiv = document.createElement('div');
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
    approveSelection(buttonId);
}

// Function to handle approval of the selection
function approveSelection(buttonId) {
    // Hide VoteScreen elements
    document.getElementById('VoteScreen').style.display = 'none';

    // Stop the countdown timer
    stopCountdown();

    // Show NewScreen elements
    document.getElementById('NewScreen').style.display = 'block';

    // Additional logic based on buttonId if needed
}

// Add event listeners to the buttons
document.getElementById('RedCard').addEventListener('click', handleButtonClick);
document.getElementById('YellowCard').addEventListener('click', handleButtonClick);
document.getElementById('GreenCard').addEventListener('click', handleButtonClick);
