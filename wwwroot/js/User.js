


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




