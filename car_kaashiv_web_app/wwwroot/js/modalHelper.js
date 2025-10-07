(function () {
    function cleanModalBackdrop(modalId) {
        const modal = document.getElementById(modalId);
        if (!modal) return;

        modal.addEventListener("hidden.bs.modal", () => {
            // Remove any leftover backdrops

            document.querySelectorAll(".modal-backdrop").forEach(b => b.remove());
            //Remove modal-opn class and padding
            document.body.classList.remove("modal-open");
            document.body.style.paddingRight = "";

        });
    }
    //Automatically attach cleanup to all modals on page
    document.addEventListener("DOMContentLoaded", () => {

        const modals = document.querySelectorAll(".modal");
        modals.forEach(modal => cleanModalBackdrop(modal.id));
    });
    //Expose manually 
    window.cleanModalBackdrop = cleanModalBackdrop;
})();