/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import type { CreateCameraDTO } from '../models/CreateCameraDTO';
import type { UpdateCameraDTO } from '../models/UpdateCameraDTO';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class CameraService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param cameraId
     * @param resolution
     * @returns any OK
     * @throws ApiError
     */
    public getApiV0CameraControllerGetStream(
        cameraId: string,
        resolution?: string,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
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
    public postApiV0CameraControllerCreateCamera(
        requestBody?: CreateCameraDTO,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
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
    public postApiV0CameraControllerRemoveCamera(
        cameraId: string,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
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
    public postApiV0CameraControllerUpdateCamera(
        requestBody?: UpdateCameraDTO,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'POST',
            url: '/API/v0/CameraController/UpdateCamera',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
