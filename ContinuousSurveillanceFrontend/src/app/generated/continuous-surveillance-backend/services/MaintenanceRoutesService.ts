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
export class MaintenanceRoutesService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherMaintenanceAvailabilityCheck(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Maintenance/AvailabilityCheck',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherMaintenanceCurrentVersion(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Maintenance/CurrentVersion',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiOtherMaintenanceShowAllEndpoints(): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'GET',
            url: '/API/Other/Maintenance/ShowAllEndpoints',
        });
    }
}
