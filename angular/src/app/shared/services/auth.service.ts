import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequestDto } from '../models/login-request.dto';
import { Observable } from 'rxjs';
import { LoginResponseDto } from '../models/login-response.dto';
import { environment } from '@/environments/environment';
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../constants/keys.cont';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  httpClient = inject(HttpClient);
  tokenService = inject(TokenService);

  public login(input: LoginRequestDto): Observable<LoginResponseDto> {
    var body = {
      username: input.username,
      password: input.password,
      client_id: environment?.oAuthConfig?.clientId,
      grant_type: 'password',
      scope: environment?.oAuthConfig?.scope,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent(body[key])}`)
      .join('&');
    return this.httpClient.post<LoginResponseDto>(
      environment?.oAuthConfig?.issuer + 'connect/token',
      data,
      { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
    );
  }

  public refreshToken(refreshToken: string): Observable<LoginResponseDto> {
    var body = {
      refresh_token: refreshToken,
      client_id: environment?.oAuthConfig?.clientId,
      grant_type: 'refresh_token',
      scope: environment?.oAuthConfig?.scope,
    };

    const data = Object.keys(body)
      .map((key, index) => `${key}=${encodeURIComponent(body[key])}`)
      .join('&');
    return this.httpClient.post<LoginResponseDto>(
      environment?.oAuthConfig?.issuer + 'connect/token',
      data,
      { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
    );
  }

  public isAuthenticated(): boolean {
    return this.tokenService.getToken() != null;
  }
  public logout() {
    this.tokenService.signOut();
  }
}
