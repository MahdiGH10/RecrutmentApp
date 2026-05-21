document.addEventListener("DOMContentLoaded", () => {
	document.querySelectorAll("[data-auto-submit]").forEach((element) => {
		element.addEventListener("change", () => element.form?.submit());
	});

	const toasts = document.querySelectorAll("[data-toast]");
	toasts.forEach((toast) => {
		const closeButton = toast.querySelector("[data-toast-close]");
		const timeoutId = window.setTimeout(() => {
			toast.classList.add("fade-out");
			window.setTimeout(() => toast.remove(), 350);
		}, 4500);

		closeButton?.addEventListener("click", () => {
			window.clearTimeout(timeoutId);
			toast.remove();
		});
	});

	document.querySelectorAll("[data-account-type-grid]").forEach((grid) => {
		const cards = grid.querySelectorAll(".account-type-card");
		const form = grid.closest("form");
		const recruiterFields = form?.querySelector("[data-recruiter-fields]");
		const syncSelection = () => {
			let selectedValue = "";
			cards.forEach((card) => {
				const input = card.querySelector("input");
				const isSelected = Boolean(input?.checked);
				card.classList.toggle("is-selected", isSelected);
				if (isSelected) {
					selectedValue = input.value;
				}
			});

			if (recruiterFields) {
				recruiterFields.hidden = selectedValue !== "Recruteur";
			}
		};

		grid.addEventListener("change", syncSelection);
		syncSelection();
	});
});

window.recruitAppToast = function (message) {
	console.log(message);
};
