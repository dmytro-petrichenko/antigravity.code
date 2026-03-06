export function observeRatioViewportFrameContentSize(frameElement, contentElement) {
    if (!frameElement) {
        throw new Error("frameElement is required.");
    }

    if (!contentElement) {
        throw new Error("contentElement is required.");
    }

    const format = (value) => Number.isFinite(value) ? Number(value.toFixed(2)) : value;

    let lastFrameWidth = null;
    let lastContentWidth = null;
    let resizeRafId = 0;

    const readSnapshot = () => {
        const frameRect = frameElement.getBoundingClientRect();
        const contentRect = contentElement.getBoundingClientRect();

        return {
            frameRect,
            contentRect,
            frameWidth: format(frameRect.width),
            frameHeight: format(frameRect.height),
            frameRatio: format(frameRect.width / Math.max(frameRect.height, 1)),
            contentWidth: format(contentRect.width),
            contentHeight: format(contentRect.height),
            contentRatio: format(contentRect.width / Math.max(contentRect.height, 1))
        };
    };

    const logWidthChange = (source, force = false) => {
        const snapshot = readSnapshot();

        const frameWidthChanged = lastFrameWidth !== snapshot.frameWidth;
        const contentWidthChanged = lastContentWidth !== snapshot.contentWidth;

        if (!force && !frameWidthChanged && !contentWidthChanged) {
            return;
        }

        console.log("[RatioViewportFrame] width change", {
            source,
            frameWidthChanged,
            contentWidthChanged,
            previousFrameWidth: lastFrameWidth,
            previousContentWidth: lastContentWidth,
            frameWidth: snapshot.frameWidth,
            contentWidth: snapshot.contentWidth,
            contentHeight: snapshot.contentHeight,
            contentRatio: snapshot.contentRatio,
            frameHeight: snapshot.frameHeight,
            frameRatio: snapshot.frameRatio
        });

        lastFrameWidth = snapshot.frameWidth;
        lastContentWidth = snapshot.contentWidth;
    };

    const observer = new ResizeObserver(() => {
        logWidthChange("resize-observer");
    });

    observer.observe(contentElement);
    observer.observe(frameElement);

    const onWindowResize = () => {
        if (resizeRafId) {
            cancelAnimationFrame(resizeRafId);
        }

        resizeRafId = requestAnimationFrame(() => {
            resizeRafId = 0;
            logWidthChange("window-resize");
        });
    };

    window.addEventListener("resize", onWindowResize, { passive: true });

    logWidthChange("init", true);

    return {
        dispose() {
            observer.disconnect();
            window.removeEventListener("resize", onWindowResize);

            if (resizeRafId) {
                cancelAnimationFrame(resizeRafId);
                resizeRafId = 0;
            }
        }
    };
}
