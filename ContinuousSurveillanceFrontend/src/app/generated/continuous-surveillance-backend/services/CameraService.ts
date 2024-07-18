/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CreateCameraDTO } from '../models/CreateCameraDTO';
import type { UpdateCameraDTO } from '../models/UpdateCameraDTO';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CameraService {
    /**
     * @param cameraId
     * @param resolution
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV0CameraControllerGetStream(
        cameraId: string,
        resolution?: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/v0/CameraController/GetStream/{cameraId}',
            path: {
                'cameraId': cameraId,
            },
            query: {
                'resolution': resolution,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV0CameraControllerCreateCamera(
        requestBody?: CreateCameraDTO,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/API/v0/CameraController/CreateCamera',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param cameraId
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV0CameraControllerRemoveCamera(
        cameraId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/API/v0/CameraController/RemoveCamera/{cameraId}',
            path: {
                'cameraId': cameraId,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV0CameraControllerUpdateCamera(
        requestBody?: UpdateCameraDTO,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/API/v0/CameraController/UpdateCamera',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
