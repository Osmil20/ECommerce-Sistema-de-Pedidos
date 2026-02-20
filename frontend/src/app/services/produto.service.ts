import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Produto, ProdutoCreate } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProdutoService {
  private readonly url = `${environment.apiUrl}/produtos`;

  constructor(private http: HttpClient) {}

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.url);
  }

  obterPorId(id: number): Observable<Produto> {
    return this.http.get<Produto>(`${this.url}/${id}`);
  }

  criar(dto: ProdutoCreate): Observable<Produto> {
    return this.http.post<Produto>(this.url, dto);
  }

  atualizar(id: number, dto: ProdutoCreate): Observable<Produto> {
    return this.http.put<Produto>(`${this.url}/${id}`, dto);
  }

  remover(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
