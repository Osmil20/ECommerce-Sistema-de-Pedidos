import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Cliente, Produto, ItemPedido } from '../../models/models';
import { ClienteService } from '../../services/cliente.service';
import { ProdutoService } from '../../services/produto.service';
import { PedidoService } from '../../services/pedido.service';

interface ItemSelecionado extends Produto {
  quantidade: number;
}

@Component({
  selector: 'app-novo-pedido',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Novo Pedido</h1>
      </div>

      @if (erro) { <div class="alert alert-error">{{ erro }}</div> }

      <div class="pedido-layout">
        <div class="card">
          <h2>1. Selecionar Cliente</h2>
          <select [(ngModel)]="clienteSelecionadoId" [ngModelOptions]="{standalone: true}" class="select-full">
            <option [value]="null" disabled selected>Selecione um cliente...</option>
            @for (c of clientes; track c.id) {
              <option [value]="c.id">{{ c.nome }} — {{ c.cpf }}</option>
            }
          </select>
        </div>

        <div class="card">
          <h2>2. Adicionar Produtos</h2>
          <table class="table">
            <thead>
              <tr>
                <th>Produto</th>
                <th>Preço</th>
                <th>Estoque</th>
                <th>Qtd</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (p of produtos; track p.id) {
                <tr [class.out-of-stock]="p.estoque === 0">
                  <td>{{ p.nome }}</td>
                  <td>{{ p.preco | currency:'BRL' }}</td>
                  <td>{{ p.estoque }}</td>
                  <td>
                    <input
                      type="number"
                      min="0"
                      [max]="p.estoque"
                      [(ngModel)]="quantidades[p.id]"
                      [ngModelOptions]="{standalone: true}"
                      [disabled]="p.estoque === 0"
                      class="qty-input"
                    />
                  </td>
                  <td>
                    <button
                      class="btn btn-secondary"
                      (click)="adicionarItem(p)"
                      [disabled]="p.estoque === 0 || !quantidades[p.id] || quantidades[p.id] <= 0">
                      Adicionar
                    </button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <div class="card">
          <h2>3. Revisar Pedido</h2>
          @if (itensSelecionados.length === 0) {
            <p class="empty">Nenhum produto adicionado ainda.</p>
          } @else {
            <table class="table">
              <thead>
                <tr><th>Produto</th><th>Qtd</th><th>Unit.</th><th>Subtotal</th><th></th></tr>
              </thead>
              <tbody>
                @for (item of itensSelecionados; track item.id) {
                  <tr>
                    <td>{{ item.nome }}</td>
                    <td>{{ item.quantidade }}</td>
                    <td>{{ item.preco | currency:'BRL' }}</td>
                    <td>{{ item.preco * item.quantidade | currency:'BRL' }}</td>
                    <td><button class="btn-icon btn-danger" (click)="removerItem(item.id)">✕</button></td>
                  </tr>
                }
              </tbody>
              <tfoot>
                <tr class="total-row">
                  <td colspan="3"><strong>Total</strong></td>
                  <td><strong>{{ totalPedido | currency:'BRL' }}</strong></td>
                  <td></td>
                </tr>
              </tfoot>
            </table>
          }

          <div class="form-actions" style="margin-top: 16px;">
            <button class="btn btn-secondary" (click)="voltar()">Cancelar</button>
            <button
              class="btn btn-primary"
              (click)="confirmarPedido()"
              [disabled]="!clienteSelecionadoId || itensSelecionados.length === 0 || salvando">
              {{ salvando ? 'Criando...' : 'Confirmar Pedido' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .pedido-layout { display: flex; flex-direction: column; gap: 20px; }
    .select-full { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 1rem; }
    .qty-input { width: 64px; padding: 6px; border: 1px solid #ddd; border-radius: 4px; text-align: center; }
    .out-of-stock { opacity: 0.5; }
    .total-row td { border-top: 2px solid #ddd; padding-top: 12px; }
  `]
})
export class NovoPedidoComponent implements OnInit {
  clientes: Cliente[] = [];
  produtos: Produto[] = [];
  itensSelecionados: ItemSelecionado[] = [];
  quantidades: Record<number, number> = {};
  clienteSelecionadoId: number | null = null;
  salvando = false;
  erro = '';

  constructor(
    private clienteService: ClienteService,
    private produtoService: ProdutoService,
    private pedidoService: PedidoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.clienteService.listar().subscribe(c => this.clientes = c);
    this.produtoService.listar().subscribe(p => {
      this.produtos = p;
      p.forEach(prod => this.quantidades[prod.id] = 0);
    });
  }

  get totalPedido(): number {
    return this.itensSelecionados.reduce((acc, i) => acc + i.preco * i.quantidade, 0);
  }

  adicionarItem(produto: Produto): void {
    const qty = this.quantidades[produto.id];
    if (!qty || qty <= 0) return;

    const existente = this.itensSelecionados.find(i => i.id === produto.id);
    if (existente) {
      existente.quantidade = qty;
    } else {
      this.itensSelecionados.push({ ...produto, quantidade: qty });
    }
  }

  removerItem(produtoId: number): void {
    this.itensSelecionados = this.itensSelecionados.filter(i => i.id !== produtoId);
  }

  confirmarPedido(): void {
    if (!this.clienteSelecionadoId || !this.itensSelecionados.length) return;

    this.salvando = true;
    const dto = {
      clienteId: this.clienteSelecionadoId,
      itens: this.itensSelecionados.map(i => ({ produtoId: i.id, quantidade: i.quantidade }))
    };

    this.pedidoService.criar(dto).subscribe({
      next: () => this.router.navigate(['/pedidos']),
      error: (e) => {
        this.erro = e.error?.mensagem ?? 'Erro ao criar pedido.';
        this.salvando = false;
      }
    });
  }

  voltar(): void {
    this.router.navigate(['/pedidos']);
  }
}
