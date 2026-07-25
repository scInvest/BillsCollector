window.chatAgentWindow = window.chatAgentWindow || {};

window.chatAgentWindow.registerMessageInput = function (textarea, dotNetReference) {
    if (!textarea || !dotNetReference) {
        return;
    }

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            dotNetReference.invokeMethodAsync("SubmitMessageFromKeyboard");
        }
    });
};

window.chatAgentWindow.registerTextInput = function (textarea, dotNetReference) {
    if (!textarea || !dotNetReference) {
        return;
    }

    textarea.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            event.preventDefault();
            dotNetReference.invokeMethodAsync("CancelTextInput");
        } else if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault();
            dotNetReference.invokeMethodAsync("SubmitTextInput");
        }
    });
};

window.chatAgentWindow.autoResizeTextarea = function (textarea) {
    if (!textarea) {
        return;
    }

    textarea.style.height = "auto";

    const maxHeight = parseFloat(getComputedStyle(textarea).maxHeight);
    const nextHeight = Number.isFinite(maxHeight)
        ? Math.min(textarea.scrollHeight, maxHeight)
        : textarea.scrollHeight;

    textarea.style.height = `${nextHeight}px`;
    textarea.style.overflowY = textarea.scrollHeight > nextHeight ? "auto" : "hidden";
};