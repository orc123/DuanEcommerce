import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequestDto } from '../models/login-request.dto';
import { Observable } from 'rxjs';
import { LoginResponseDto } from '../models/login-response.dto';
import { environment } from '@/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  httpClient = inject(HttpClient);

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
}
