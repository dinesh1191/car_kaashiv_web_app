

function showAlertModal(message, type = "primary") {
    const modalElement = document.getElementById("alertModal");
    const modalBody = document.getElementById("alertModalBody");
    const modalHeader = document.getElementById("alertModalHeader");

    const modalInstance = bootstrap.Modal.getOrCreateInstance(modalElement);

    // Normalize and map type to Bootstrap class
    const typeMap = {
        success: "bg-success",
        error: "bg-danger",
        danger: "bg-danger",
        warning: "bg-warning",
        info: "bg-info",
        primary: "bg-primary"
    };
    const bgClass = typeMap[type.toLowerCase()] || "bg-primary";
    // Reset and apply header color
    modalHeader.classList.remove("bg-primary", "bg-success", "bg-danger", "bg-warning", "bg-info");
    modalHeader.classList.add(bgClass);

    // Set message BEFORE showing modal
    modalBody.innerText = message;    
    // Show modal
    modalInstance.show();
    // Auto-dismiss after specified duration
    setTimeout(() => {
        modalInstance.hide();
    }, 3000);
  
}


