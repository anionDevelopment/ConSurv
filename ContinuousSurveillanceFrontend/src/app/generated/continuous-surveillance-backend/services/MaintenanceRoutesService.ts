/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class MaintenanceRoutesService {
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherMaintenanceAvailabilityCheck(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Maintenance/AvailabilityCheck',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherMaintenanceCurrentVersion(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Maintenance/CurrentVersion',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherMaintenanceShowAllEndpoints(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Maintenance/ShowAllEndpoints',
        });
    }
}
