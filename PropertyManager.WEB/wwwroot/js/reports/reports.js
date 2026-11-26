(function () {
    const form = document.getElementById("report-form");
    if (!form) {
        return;
    }

    const storageKey = "pm.reports.common";
    const preset = form.querySelector("[data-period-preset]");
    const start = form.querySelector("[data-start-date]");
    const end = form.querySelector("[data-end-date]");
    const dates = form.querySelector("[data-period-dates]");
    const summary = form.querySelector("[data-period-summary]");
    const buildings = form.querySelector("[data-buildings]");
    const buildingsToggle = buildings && buildings.querySelector("[data-buildings-toggle]");
    const buildingsPanel = buildings && buildings.querySelector("[data-buildings-panel]");
    const buildingsSearch = buildings && buildings.querySelector("[data-buildings-search]");
    const buildingsClear = buildings && buildings.querySelector("[data-buildings-clear]");
    const buildingsSummary = buildings && buildings.querySelector("[data-buildings-summary]");
    const buildingsEmpty = buildings && buildings.querySelector("[data-buildings-empty]");
    const reportKey = document.getElementById("reportKey");

    function iso(date) {
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return date.getFullYear() + "-" + month + "-" + day;
    }

    function endOfMonth(year, month) {
        return new Date(year, month + 1, 0);
    }

    function rangeFor(value) {
        const today = new Date();
        const year = today.getFullYear();
        const month = today.getMonth();
        if (value === "thisMonth") {
            return [new Date(year, month, 1), endOfMonth(year, month)];
        }
        if (value === "lastMonth") {
            const first = new Date(year, month - 1, 1);
            return [first, endOfMonth(first.getFullYear(), first.getMonth())];
        }
        if (value === "thisYear") {
            return [new Date(year, 0, 1), new Date(year, 11, 31)];
        }
        if (value === "lastYear") {
            return [new Date(year - 1, 0, 1), new Date(year - 1, 11, 31)];
        }
        return null;
    }

    function formatDisplay(isoDate) {
        if (!isoDate) {
            return "";
        }
        const parts = isoDate.split("-").map(Number);
        return new Date(parts[0], parts[1] - 1, parts[2]).toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric"
        });
    }

    function applyPreset(value) {
        const range = rangeFor(value);
        const custom = value === "custom";
        if (dates) {
            dates.classList.toggle("d-none", !custom);
        }
        if (summary) {
            summary.classList.toggle("d-none", custom);
        }
        if (range && start && end) {
            start.value = iso(range[0]);
            end.value = iso(range[1]);
        }
        if (summary && start && end) {
            summary.textContent = formatDisplay(start.value) + " – " + formatDisplay(end.value);
        }
    }

    function buildingOptions() {
        if (!buildings) {
            return [];
        }
        return Array.from(buildings.querySelectorAll("[data-buildings-option]"));
    }

    function selectedBuildings() {
        return buildingOptions().filter(function (option) {
            return option.checked;
        }).map(function (option) {
            return option.value;
        });
    }

    function refreshBuildings() {
        if (!buildingsSummary) {
            return;
        }
        const count = selectedBuildings().length;
        buildingsSummary.textContent = count === 1 ? "1 selected" : count + " selected";
        if (buildingsClear) {
            buildingsClear.disabled = count === 0;
        }
    }

    function filterBuildings() {
        const query = (buildingsSearch ? buildingsSearch.value : "").trim().toLowerCase();
        let visible = 0;
        buildingOptions().forEach(function (option) {
            const row = option.closest("[data-buildings-row]");
            const match = (option.dataset.label || "").toLowerCase().indexOf(query) !== -1;
            if (row) {
                row.hidden = !match;
            }
            if (match) {
                visible += 1;
            }
        });
        if (buildingsEmpty) {
            buildingsEmpty.hidden = visible !== 0;
        }
    }

    function setBuildingsOpen(open) {
        if (!buildingsPanel || !buildingsToggle) {
            return;
        }
        buildingsPanel.hidden = !open;
        buildingsToggle.setAttribute("aria-expanded", open ? "true" : "false");
        if (open && buildingsSearch) {
            buildingsSearch.value = "";
            filterBuildings();
            buildingsSearch.focus();
        }
    }

    function save() {
        localStorage.setItem(storageKey, JSON.stringify({
            preset: preset ? preset.value : "custom",
            startDate: start ? start.value : "",
            endDate: end ? end.value : "",
            buildingIds: selectedBuildings()
        }));
    }

    function restore() {
        const raw = localStorage.getItem(storageKey);
        if (!raw) {
            applyPreset(preset ? preset.value : "thisMonth");
            return;
        }

        try {
            const state = JSON.parse(raw);
            if (preset && state.preset) {
                preset.value = state.preset;
            }
            if (state.preset === "custom") {
                if (start && state.startDate) {
                    start.value = state.startDate;
                }
                if (end && state.endDate) {
                    end.value = state.endDate;
                }
                applyPreset("custom");
            } else {
                applyPreset(state.preset || "thisMonth");
            }
            if (buildings && Array.isArray(state.buildingIds)) {
                const selected = new Set(state.buildingIds.map(String));
                buildingOptions().forEach(function (option) {
                    option.checked = selected.has(option.value);
                });
            }
            refreshBuildings();
        } catch (error) {
            applyPreset(preset ? preset.value : "thisMonth");
        }
    }

    function selectReport(key, resetSpecific) {
        if (reportKey) {
            reportKey.value = key;
        }
        document.querySelectorAll("[data-report-panel]").forEach(function (panel) {
            const on = panel.dataset.reportPanel === key;
            panel.hidden = !on;
            panel.querySelectorAll("input, select, textarea, button").forEach(function (element) {
                element.disabled = !on;
                if (on && resetSpecific && element.dataset.default != null) {
                    element.value = element.dataset.default;
                }
            });
        });
        document.querySelectorAll("[data-select-report]").forEach(function (button) {
            button.classList.toggle("active", button.dataset.selectReport === key);
        });
        document.querySelectorAll("[data-common-for]").forEach(function (block) {
            const keys = (block.dataset.commonFor || "").split(/\s+/);
            block.hidden = keys.indexOf(key) === -1;
        });
    }

    document.querySelectorAll("[data-select-report]").forEach(function (button) {
        button.addEventListener("click", function () {
            selectReport(button.dataset.selectReport, true);
        });
    });

    if (buildingsToggle) {
        buildingsToggle.addEventListener("click", function () {
            setBuildingsOpen(buildingsPanel.hidden);
        });
    }

    if (buildingsSearch) {
        buildingsSearch.addEventListener("input", filterBuildings);
    }

    if (buildingsClear) {
        buildingsClear.addEventListener("click", function () {
            buildingOptions().forEach(function (option) {
                option.checked = false;
            });
            refreshBuildings();
            save();
        });
    }

    if (buildings) {
        buildings.addEventListener("change", function () {
            refreshBuildings();
            save();
        });
        document.addEventListener("click", function (event) {
            if (!buildings.contains(event.target)) {
                setBuildingsOpen(false);
            }
        });
        buildings.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                setBuildingsOpen(false);
                if (buildingsToggle) {
                    buildingsToggle.focus();
                }
            }
        });
    }

    if (preset) {
        preset.addEventListener("change", function () {
            applyPreset(preset.value);
            save();
        });
    }

    [start, end].forEach(function (element) {
        if (!element) {
            return;
        }
        element.addEventListener("change", save);
    });

    form.addEventListener("submit", function () {
        if (preset && preset.value !== "custom") {
            applyPreset(preset.value);
        }
        save();
        const button = form.querySelector("[data-generate]:not([disabled])") || form.querySelector("[data-generate]");
        if (button) {
            button.disabled = true;
            button.setAttribute("aria-busy", "true");
            button.textContent = "Generating...";
        }
    });

    refreshBuildings();
    restore();
    if (reportKey && reportKey.value) {
        selectReport(reportKey.value, false);
    }
})();
