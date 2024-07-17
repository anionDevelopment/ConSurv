/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CommonRoutesService {
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherResourcesInformationTermsOfService(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Resources/Information/TermsOfService',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherResourcesInformationContact(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Resources/Information/Contact',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiOtherResourcesInformationLicense(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/API/Other/Resources/Information/License',
        });
    }
}
