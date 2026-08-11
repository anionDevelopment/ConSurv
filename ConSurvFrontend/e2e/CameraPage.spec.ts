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
    test('looks like the baseline-screenshot', async ({ page }) => {
        await simulateLoggedInUser(page);
        await simulateCameras(page);
        await expectPageToLookLikeBaseline(page, `/user/camera?cameraId=${idOfTheCameraWithAnOwnPage}`, 'camera-page', [selectorOfTheVideoPlayer]);
    });
});
