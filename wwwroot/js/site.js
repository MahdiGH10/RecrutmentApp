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

	document.querySelectorAll("[data-password-strength]").forEach((meter) => {
		const input = meter.parentElement?.querySelector("[data-password-strength-input]");
		const fill = meter.querySelector("[data-password-strength-fill]");
		const text = meter.querySelector("[data-password-strength-text]");

		const evaluate = (value) => {
			let score = 0;
			if (value.length >= 8) score += 1;
			if (/[a-z]/.test(value) && /[A-Z]/.test(value)) score += 1;
			if (/\d/.test(value)) score += 1;
			if (/[^A-Za-z0-9]/.test(value)) score += 1;

			const labels = [
				{ min: 0, label: "Très faible", width: "12%", cls: "is-weak" },
				{ min: 2, label: "Faible", width: "35%", cls: "is-weak" },
				{ min: 3, label: "Moyen", width: "65%", cls: "is-medium" },
				{ min: 4, label: "Fort", width: "100%", cls: "is-strong" }
			];

			const match = labels.slice().reverse().find((item) => score >= item.min) || labels[0];
			fill.style.width = match.width;
			fill.classList.remove("is-medium", "is-strong");
			if (match.cls === "is-medium") {
				fill.classList.add("is-medium");
			} else if (match.cls === "is-strong") {
				fill.classList.add("is-strong");
			}
			text.textContent = `${match.label} — le mot de passe doit contenir des lettres, des chiffres et des symboles.`;
		};

		if (input) {
			input.addEventListener("input", (event) => evaluate(event.target.value));
			evaluate(input.value ?? "");
		}
	});
});

window.recruitAppToast = function (message) {
	console.log(message);
};
