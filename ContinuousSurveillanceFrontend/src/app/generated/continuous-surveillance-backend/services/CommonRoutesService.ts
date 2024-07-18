/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import type { Observable } from 'rxjs';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
@Injectable({
    providedIn: 'root',
})
export class CommonRoutesService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherResourcesInformationTermsOfService(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Resources/Information/TermsOfService',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherResourcesInformationContact(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Resources/Information/Contact',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherResourcesInformationLicense(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Resources/Information/License',
        });
    }
}
