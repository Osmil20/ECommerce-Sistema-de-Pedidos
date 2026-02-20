import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Pedido } from '../../models/models';
import { PedidoService } from '../../services/pedido.service';

@Component({
  selector: 'app-pedidos',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Pedidos</h1>
        <a routerLink="/pedidos/novo" class="btn btn-primary">+ Novo Pedido</a>
      </div>

      <div class="filters">
        <button [class]="filtro === null ? 'btn btn-primary' : 'btn btn-secondary'" (click)="filtrar(null)">Todos</button>
        <button [class]="filtro === 'Criado' ? 'btn btn-primary' : 'btn btn-secondary'" (click)="filtrar('Criado')">Criados</button>
        <button [class]="filtro === 'Pago' ? 'btn btn-primary' : 'btn btn-secondary'" (click)="filtrar('Pago')">Pagos</button>
        <button [class]="filtro === 'Cancelado' ? 'btn btn-primary' : 'btn btn-secondary'" (click)="filtrar('Cancelado')">Cancelados</button>
      </div>

      @if (erro) { <div class="alert alert-error">{{ erro }}</div> }
      @if (sucesso) { <div class="alert alert-success">{{ sucesso }}</div> }

      <div class="cards-grid">
        @for (p of pedidos; track p.id) {
          <div class="pedido-card card">
            <div class="pedido-header">
              <span class="pedido-id">#{{ p.id }}</span>
              <span [class]="'badge badge-' + statusClass(p.status)">{{ p.status }}</span>
            </div>
            <div class="pedido-body">
              <p class="pedido-cliente">👤 {{ p.nomeCliente }}</p>
              <p class="pedido-data">📅 {{ p.dataPedido | date:'dd/MM/yyyy HH:mm' }}</p>
              <p class="pedido-total">💰 {{ p.valorTotal | currency:'BRL' }}</p>
              <ul class="pedido-itens">
                @for (item of p.itens; track item.produtoId) {
                  <li>{{ item.nomeProduto }} × {{ item.quantidade }} — {{ item.subtotal | currency:'BRL' }}</li>
                }
              </ul>
            </div>
            @if (p.status === 'Criado') {
              <div class="pedido-actions">
                <button class="btn btn-success" (click)="pagar(p.id)">✓ Confirmar Pagamento</button>
                <button class="btn btn-danger" (click)="cancelar(p.id)">✕ Cancelar</button>
              </div>
            }
          </div>
        }
        @if (pedidos.length === 0) {
          <div class="empty-state">
            <p>Nenhum pedido encontrado.</p>
            <a routerLink="/pedidos/novo" class="btn btn-primary">Criar primeiro pedido</a>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .filters { display: flex; gap: 8px; margin-bottom: 24px; }
    .cards-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 20px; }
    .pedido-card { padding: 20px; }
    .pedido-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
    .pedido-id { font-weight: 700; font-size: 1.1rem; color: #333; }
    .pedido-body p { margin: 4px 0; font-size: 0.9rem; color: #555; }
    .pedido-cliente { font-size: 1rem !important; font-weight: 600; color: #222 !important; }
    .pedido-total { font-size: 1.1rem !important; font-weight: 700; color: #1565c0 !important; }
    .pedido-itens { margin: 8px 0 0; padding-left: 16px; font-size: 0.85rem; color: #666; }
    .pedido-itens li { margin: 2px 0; }
    .pedido-actions { display: flex; gap: 8px; margin-top: 16px; border-top: 1px solid #eee; padding-top: 16px; }
    .empty-state { grid-column: 1/-1; text-align: center; padding: 60px; color: #888; }
  `]
})
export class PedidosComponent implements OnInit {
  pedidos: Pedido[] = [];
  filtro: string | null = null;
  erro = '';
  sucesso = '';

  constructor(private service: PedidoService) {}

  ngOnInit(): void { this.carregar(); }

  carregar(): void {
    this.service.listar(this.filtro ?? undefined).subscribe({ next: p => this.pedidos = p });
  }

  filtrar(status: string | null): void {
    this.filtro = status;
    this.carregar();
  }

  pagar(id: number): void {
    this.service.pagar(id).subscribe({
      next: () => { this.sucesso = 'Pedido pago!'; this.carregar(); setTimeout(() => this.sucesso = '', 3000); },
      error: (e) => { this.erro = e.error?.mensagem ?? 'Erro ao pagar.'; }
    });
  }

  cancelar(id: number): void {
    if (!confirm('Cancelar este pedido?')) return;
    this.service.cancelar(id).subscribe({
      next: () => { this.sucesso = 'Pedido cancelado.'; this.carregar(); setTimeout(() => this.sucesso = '', 3000); },
      error: (e) => { this.erro = e.error?.mensagem ?? 'Erro ao cancelar.'; }
    });
  }

  statusClass(status: string): string {
    return status === 'Pago' ? 'success' : status === 'Cancelado' ? 'error' : 'warning';
  }
}
