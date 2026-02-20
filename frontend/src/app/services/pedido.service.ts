import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Pedido, PedidoCreate } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PedidoService {
  private readonly url = `${environment.apiUrl}/pedidos`;

  constructor(private http: HttpClient) {}

  listar(status?: string): Observable<Pedido[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    return this.http.get<Pedido[]>(this.url, { params });
  }

  obterPorId(id: number): Observable<Pedido> {
    return this.http.get<Pedido>(`${this.url}/${id}`);
  }

  criar(dto: PedidoCreate): Observable<Pedido> {
    return this.http.post<Pedido>(this.url, dto);
  }

  pagar(id: number): Observable<Pedido> {
    return this.http.patch<Pedido>(`${this.url}/${id}/pagar`, {});
  }

  cancelar(id: number): Observable<Pedido> {
    return this.http.patch<Pedido>(`${this.url}/${id}/cancelar`, {});
  }
}
