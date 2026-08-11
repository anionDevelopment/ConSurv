import { test } from '@playwright/test';
import { expectPageToLookLikeBaseline } from './support/VisualRegression';
import { simulateLoggedInUser } from './support/AuthenticatedSession';
import { simulateCameras } from './support/SimulatedCameras';

test.describe('Cameras-list-page', () => {
    test('looks like the baseline-screenshot', async ({ page }) => {
        await simulateLoggedInUser(page);
        await simulateCameras(page);
        await expectPageToLookLikeBaseline(page, '/user/cameras', 'cameras-list-page');
    });
});
