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
export class UserService {
    constructor(public readonly http: HttpClient) {}
    /**
     * @param user
     * @param password
     * @returns string OK
     * @throws ApiError
     */
    public putApiV0UserControllerCreateUser(
        user?: string,
        password?: string,
    ): Observable<string> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/API/v0/UserController/CreateUser',
            headers: {
                'user': user,
                'password': password,
            },
        });
    }
    /**
     * @param user
     * @param password
     * @returns string OK
     * @throws ApiError
     */
    public putApiV0UserControllerLogin(
        user?: string,
        password?: string,
    ): Observable<string> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/API/v0/UserController/Login',
            headers: {
                'user': user,
                'password': password,
            },
        });
    }
    /**
     * @param token
     * @returns any OK
     * @throws ApiError
     */
    public putApiV0UserControllerLogout(
        token?: string,
    ): Observable<any> {
        return __request(OpenAPI, this.http, {
            method: 'PUT',
            url: '/API/v0/UserController/Logout',
            headers: {
                'token': token,
            },
        });
    }
}
