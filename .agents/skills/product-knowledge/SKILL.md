---
name: product-knowledge
description: What ConSurv is, how this repository is structured and which mechanisms exist for building, testing and visual-regression-testing it. Use this before fixing a defect or developing a feature in this repository, to know where things belong and how to verify a change.
---

# ConSurv

ConSurv (short for "continuous surveillance") is an ip-camera-management-system. It records the streams of
ip-cameras, shows them live in the browser, can blacken parts of a video and uses the gpu when one is
available. The camera-streams are read via rtsp.

## Structure of the repository

The repository follows the "common project structure": all sourcecode lives in code-units, and every code-unit
has its own `Other`-folder with its build-, quality-check- and reference-files. Use the
`work-with-common-project-structure`-skill when you need the details of that structure.

The code-units are:

- `ConSurvBackend` — the dotnet-api-server. It owns the cameras, the recordings and the users, and it is the
  only part which talks to the database and to the cameras.
- `ConSurvFrontend` — the angular-web-application. It only talks to the backend.
- `ConSurvApp` — the smartphone-application.
- `ConSurv` — the composition of the parts (container-images, example-deployments).

The folder `ConSurvFrontend/src/app/generated` is generated from the api-specification of the backend. Never
edit anything in it: the next build overwrites it. If something is wrong there, fix it in the backend and
regenerate. To regenerate only, run `Build.py` of the backend-code-unit and afterwards `CommonTasks.py` of the
frontend-code-unit; a full `task bb` also does it but takes far longer.

## Building

`scbuildcodeunits` builds everything: compile, unit-tests, linting, security-checks and the visual-regression-
tests. The task `task bb` (`BaseBuildAllCodeunits`) does the same. Both take a while — half an hour is normal.

For a single code-unit, run the scripts in `<code-unit>/Other/Build` and `<code-unit>/Other/QualityCheck`, and
run them from the folder they are located in.

## Testing

- **Unit-tests** live next to the code-unit (`ConSurvBackendTests`, and the `*.spec.ts`-files of the frontend).
  They are executed by `RunTestcases.py` of the respective code-unit.
- **Visual-regression-tests** check that the rendered pages do not change unintentionally. They are described
  below, because they have some properties which are not obvious.

### Visual-regression-tests

They live in `ConSurvFrontend/e2e` and are written with playwright.

- **They always run in a container** (`mcr.microsoft.com/playwright`, the version is defined in
  `.ScriptCollection/OCIImages/ImageDefinition.csv`). A screenshot depends on the operating-system (font-
  rendering, available fonts, rendering of form-controls), and that difference is far bigger than the tolerance
  of the comparison. Therefore only linux-baselines exist, under
  `ConSurvFrontend/Other/Resources/VisualRegressionBaselines/<browser>/linux/`.
- **A testcase is written once and executed by three browsers** (chromium, firefox, webkit). Every browser has
  its own baseline-screenshot.
- **Generate baselines**: run `UpdateVisualRegressionBaselines.py` in `ConSurvFrontend/Other/QualityCheck`.
  Always run `RunTestcases.py` afterwards: newly generated baselines say nothing about whether the page is
  rendered reproducibly, and only the comparison shows that.
- **When a testcase fails**, the expected, the actual and the differing screenshot are stored in
  `ConSurvFrontend/Other/Artifacts/VisualRegressionTestFailures/<timestamp>/`. That folder is the first place
  to look, and it is ignored by git.
- **No backend is required.** The testcases answer the requests of the application themselves
  (`e2e/support/SimulatedBackend.ts` and the files which use it), so the screenshots do not depend on the
  content of a database. Everything a page displays has to be a fixed value; anything which differs per run
  has to be masked.
- **`expectPageToLookLikeBaseline`** in `e2e/support/VisualRegression.ts` is the entry-point of every testcase.
  It opens the page, waits until it is really rendered and compares it. Tolerance, viewport, timezone and
  color-scheme are defined centrally in `playwright.config.ts`. Its last parameter takes selectors of elements
  which are only volatile on one page; the footer (which contains the version) is masked on every page.
- **Two further checks compare the browsers with each other** (`check_screenshots_are_similar` and
  `check_layouts_are_similar`, called by `RunTestcases.py`). A baseline-screenshot is only ever compared with
  the screenshot of its own browser, so a page which looks different in one browser than in the others would
  stay unnoticed forever. The relevant one of the two compares the geometry (position and size of every
  element, written by `e2e/support/PageLayout.ts` while the tests run) and not the pixels: the engines
  rasterize text differently, so even a page which looks identical differs in thousands of pixels.

If one of those two checks reports a difference, that is a defect of the page and not of the testcase. Two
examples which were found this way and fixed: a line-break inside a flex-container (which the engines give a
different height, so a menu was 20 pixels higher in firefox) and an inline-element which contains a
block-element (which made an element 28 pixels higher in webkit).

## Things which are easy to get wrong

- The fonts are delivered with the application (`@fontsource`-packages, see `angular.json`) and are not loaded
  from an external font-provider. Do not change that back: an externally loaded font is requested with
  `display=swap`, so the browser renders with a replacement-font first and repaints afterwards — which made a
  visual-regression-testcase fail sporadically, and it additionally made every client contact a third party.
- The name of the product in the user-interface comes from `Settings.getAppName()` and not from the name of the
  npm-package: an npm-package-name has to be lowercase and without spaces.
