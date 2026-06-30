window.chatAgentWindow = window.chatAgentWindow || {};

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