import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Produto } from '../../models/models';
import { ProdutoService } from '../../services/produto.service';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="page">
      <div class="page-header">
        <h1>Produtos</h1>
        <button class="btn btn-primary" (click)="abrirFormulario()">+ Novo Produto</button>
      </div>

      @if (exibirForm) {
        <div class="card form-card">
          <h2>{{ editandoId ? 'Editar Produto' : 'Novo Produto' }}</h2>
          <form [formGroup]="form" (ngSubmit)="salvar()">
            <div class="form-row">
              <div class="form-group">
                <label>Nome *</label>
                <input formControlName="nome" placeholder="Nome do produto" />
              </div>
              <div class="form-group">
                <label>Preço *</label>
                <input formControlName="preco" type="number" step="0.01" placeholder="0,00" />
              </div>
              <div class="form-group">
                <label>Estoque *</label>
                <input formControlName="estoque" type="number" placeholder="0" />
              </div>
            </div>
            <div class="form-group">
              <label>Descrição</label>
              <input formControlName="descricao" placeholder="Descrição do produto" />
            </div>
            <div class="form-actions">
              <button type="button" class="btn btn-secondary" (click)="cancelar()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="form.invalid">
                {{ editandoId ? 'Salvar' : 'Cadastrar' }}
              </button>
            </div>
          </form>
        </div>
      }

      @if (erro) { <div class="alert alert-error">{{ erro }}</div> }
      @if (sucesso) { <div class="alert alert-success">{{ sucesso }}</div> }

      <div class="card">
        <table class="table">
          <thead>
            <tr>
              <th>#</th>
              <th>Nome</th>
              <th>Descrição</th>
              <th>Preço</th>
              <th>Estoque</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            @for (p of produtos; track p.id) {
              <tr>
                <td>{{ p.id }}</td>
                <td><strong>{{ p.nome }}</strong></td>
                <td>{{ p.descricao }}</td>
                <td>{{ p.preco | currency:'BRL' }}</td>
                <td>
                  <span [class]="p.estoque === 0 ? 'badge badge-error' : p.estoque <= 5 ? 'badge badge-warning' : 'badge badge-success'">
                    {{ p.estoque }} un.
                  </span>
                </td>
                <td>
                  <button class="btn-icon" (click)="editar(p)" title="Editar">✏️</button>
                  <button class="btn-icon btn-danger" (click)="remover(p.id)" title="Remover">🗑️</button>
                </td>
              </tr>
            }
            @if (produtos.length === 0) {
              <tr><td colspan="6" class="empty">Nenhum produto cadastrado.</td></tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class ProdutosComponent implements OnInit {
  produtos: Produto[] = [];
  form!: FormGroup;
  exibirForm = false;
  editandoId: number | null = null;
  erro = '';
  sucesso = '';

  constructor(private service: ProdutoService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      nome: ['', Validators.required],
      preco: [null, [Validators.required, Validators.min(0.01)]],
      estoque: [0, [Validators.required, Validators.min(0)]],
      descricao: ['']
    });
    this.carregar();
  }

  carregar(): void {
    this.service.listar().subscribe({ next: p => this.produtos = p });
  }

  abrirFormulario(): void {
    this.editandoId = null;
    this.form.reset({ estoque: 0 });
    this.exibirForm = true;
  }

  editar(p: Produto): void {
    this.editandoId = p.id;
    this.form.patchValue(p);
    this.exibirForm = true;
  }

  salvar(): void {
    if (this.form.invalid) return;
    const dto = this.form.value;

    const obs = this.editandoId
      ? this.service.atualizar(this.editandoId, dto)
      : this.service.criar(dto);

    obs.subscribe({
      next: () => { this.sucesso = this.editandoId ? 'Produto atualizado!' : 'Produto cadastrado!'; this.cancelar(); this.carregar(); },
      error: () => { this.erro = 'Erro ao salvar produto.'; }
    });
  }

  cancelar(): void {
    this.exibirForm = false;
    this.editandoId = null;
    this.form.reset();
  }

  remover(id: number): void {
    if (!confirm('Remover este produto?')) return;
    this.service.remover(id).subscribe({
      next: () => { this.sucesso = 'Produto removido!'; this.carregar(); },
      error: () => { this.erro = 'Não foi possível remover.'; }
    });
  }
}
