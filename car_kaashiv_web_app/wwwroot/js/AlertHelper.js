function showAlertModal(message, type = "primary") {
    const modalEl = document.getElementById("alertModal");
    const modalBody = document.getElementById("alertModalBody");
    const modalHeader = document.getElementById("alertModalHeader");

    // Get or create Bootstrap modal instance
    const alertModal = bootstrap.Modal.getOrCreateInstance(modalEl);

    // Map alert types to Bootstrap background classes
    const typeClassMap = {
        success: "bg-success",
        error: "bg-danger",
        danger: "bg-danger",
        warning: "bg-warning",
        info: "bg-info",
        primary: "bg-primary"
    };

    // Determine the background class based on type
    const bgClass = typeClassMap[type.toLowerCase()] || "bg-primary";

    // Reset header background classes
    modalHeader.classList.remove("bg-primary", "bg-success", "bg-danger", "bg-warning", "bg-info");
    modalHeader.classList.add(bgClass);

    // Set the alert message
    modalBody.innerText = message;

    // Show the modal
    alertModal.show();

    // Auto-hide after 3 seconds
    setTimeout(() => {
        alertModal.hide();
    }, 3000);

    // Cleanup lingering backdrop and scroll lock
    modalEl.addEventListener("hidden.bs.modal", () => {
        document.querySelectorAll(".modal-backdrop").forEach(backdrop => backdrop.remove());
        document.body.classList.remove("modal-open");
        document.body.style.paddingRight = "";
    });
}



window.showToast = function (message, type = "success") {
    const toastEl = document.getElementById("globalToast");
    const toastBody = toastEl.querySelector(".toast-body");

    const typeClassMap = {
        success: "bg-success",
        error: "bg-danger",
        warning: "bg-warning",
        info: "bg-info"
    };

    toastEl.className = "toast align-items-center border-0 text-white";
    toastEl.classList.add(typeClassMap[type] || "bg-secondary");

    toastBody.textContent = message;

    const toast = new bootstrap.Toast(toastEl);
    toast.show();

    setTimeout(() => toast.hide(), 3000);
};







