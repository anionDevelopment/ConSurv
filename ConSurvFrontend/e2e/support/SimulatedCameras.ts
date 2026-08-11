import { Page } from '@playwright/test';
import { respondWith } from './SimulatedBackend';

/*
 * A one-pixel png in a fixed color. It is used as preview-image of the simulated cameras.
 * The image is scaled up by the user-interface, but because it consists of exactly one pixel the
 * result is a plain area of that color and therefore contains no scaling-artefacts which could
 * differ between the browsers.
 */
const previewImageAsBase64: string = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAADElEQVR42mOYt+IIAAPyAgtr2afYAAAAAElFTkSuQmCC';

/*
 * The camera whose page is checked by the visual-regression-tests. It supports the
 * onvif-commands, so its page also contains the controls for moving and zooming the camera.
 */
const cameraWithAnOwnPage = {
    cameraId: '00000000-0000-0000-0000-000000000201',
    name: 'Entrance',
    videoInformationDTO: {
        streamURL: 'rtsp://camera.example.local/entrance',
        supportsPTZViaONVIF: true,
        onvifUrl: 'http://camera.example.local/onvif',
        onvifUsername: 'Exampleuser',
        onvifPassword: null
    },
    recordModeDTO: { recordMode: 'RecordAlways' },
    recordStateDTO: { recordState: 'CurrentlyRecording' }
};

/*
 * A second camera which differs from the first one in every value which the camera-list displays,
 * so that both variants of the record-mode- and the record-state-indicator are covered.
 */
const cameraWhichIsNotRecording = {
    cameraId: '00000000-0000-0000-0000-000000000202',
    name: 'Parking-lot',
    videoInformationDTO: {
        streamURL: 'rtsp://camera.example.local/parking-lot',
        supportsPTZViaONVIF: false,
        onvifUrl: null,
        onvifUsername: null,
        onvifPassword: null
    },
    recordModeDTO: { recordMode: 'NoRecording' },
    recordStateDTO: { recordState: 'Idle' }
};

export const idOfTheCameraWithAnOwnPage: string = cameraWithAnOwnPage.cameraId;

/*
 * Makes the simulated backend answer the requests which the camera-list and the camera-page
 * send while they are rendered.
 *
 * This function has to be called before the page is opened.
 */
export async function simulateCameras(page: Page): Promise<void> {
    await respondWith(page, '**/API/v3/CameraController/Cameras', [cameraWithAnOwnPage, cameraWhichIsNotRecording]);
    await respondWith(page, `**/API/v3/CameraController/Camera/${idOfTheCameraWithAnOwnPage}`, cameraWithAnOwnPage);
    /* This endpoint does not return an object but the image itself, encoded as base64. */
    await respondWith(page, '**/API/v3/CameraController/GetPreview/*', previewImageAsBase64);
}
