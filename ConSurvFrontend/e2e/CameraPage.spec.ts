import { test } from '@playwright/test';
import { expectPageToLookLikeBaseline } from './support/VisualRegression';
import { simulateLoggedInUser } from './support/AuthenticatedSession';
import { idOfTheCameraWithAnOwnPage, simulateCameras } from './support/SimulatedCameras';

/*
 * The camera-page contains the video-player which shows the live-stream of the camera. The
 * visual-regression-tests run without a backend, so there is no stream and the player shows its
 * own loading- respectively error-state instead. That state depends on the timing of the testrun,
 * therefore the area of the player is masked and only the rest of the page is compared.
 */
const selectorOfTheVideoPlayer: string = '.videocontainer';

test.describe('Camera-page', () => {
    /*
     * Checked in both color-schemes (see the note in "UserSettingsPage.spec.ts"). The page is behind the login, so
     * the scheme is the one the logged-in user chose (answered to the theme-request after the login).
     */
    test('looks like the baseline-screenshot in the light color-scheme', async ({ page }) => {
        await simulateLoggedInUser(page, 'light');
        await simulateCameras(page);
        await expectPageToLookLikeBaseline(page, `/user/camera?cameraId=${idOfTheCameraWithAnOwnPage}`, 'light_camera-page', [selectorOfTheVideoPlayer]);
    });

    test('looks like the baseline-screenshot in the dark color-scheme', async ({ page }) => {
        await simulateLoggedInUser(page, 'dark');
        await simulateCameras(page);
        await expectPageToLookLikeBaseline(page, `/user/camera?cameraId=${idOfTheCameraWithAnOwnPage}`, 'dark_camera-page', [selectorOfTheVideoPlayer]);
    });
});
