import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeamService } from './team.service';
import { Team } from './team.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `

    <h1>⚽ Soccer App</h1>

    <h2>Crear equipo</h2>

    <div>

      <label>Nombre:</label>
      <input [(ngModel)]="newTeam.name">

      <br><br>

      <label>País:</label>
      <input [(ngModel)]="newTeam.country">

      <br><br>

      <label>Jugadores:</label>
      <input type="number" [(ngModel)]="newTeam.playersQuantity">

      <br><br>

      <label>
        <input type="checkbox" [(ngModel)]="newTeam.enabled">
        Activo
      </label>

      <br><br>

      <button *ngIf="editingTeamId === null" (click)="createTeam()">
        Crear equipo
      </button>

      <button *ngIf="editingTeamId !== null" (click)="updateTeam()">
        Actualizar equipo
      </button>

      <button *ngIf="editingTeamId !== null" (click)="cancelEdit()">
        Cancelar
      </button>

    </div>

    <hr>

    <h2>Equipos</h2>

    <p>Equipos cargados: {{ teams.length }}</p>

    <div *ngFor="let team of teams">

      <h3>{{ team.name }}</h3>

      <p>ID: {{ team.id }}</p>
      <p>País: {{ team.country }}</p>
      <p>Jugadores: {{ team.playersQuantity }}</p>
      <p>Estado: {{ team.enabled ? 'Activo' : 'Inactivo' }}</p>

      <button (click)="editTeam(team)">Editar</button>

    </div>

  `
})
export class App {

  teams: Team[] = [];

  newTeam: Team = {
    id: 0,
    name: '',
    country: '',
    playersQuantity: 11,
    enabled: true
  };

  editingTeamId: number | null = null;

  constructor(private teamService: TeamService, private cdr: ChangeDetectorRef){
      this.loadTeams();
    }

    loadTeams(): void {
      this.teamService.getTeams().subscribe({
        next: (data) => {
          this.teams = data;
          console.log('Equipos:', this.teams);
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Error al obtener equipos:', err);
        }
      });

  }

  createTeam(): void {
    console.log('Enviando equipo:', this.newTeam);
    this.teamService.createTeam(this.newTeam).subscribe({

      next: (data) => {
        console.log('Equipo creado:', data);
        this.newTeam = {
          id: 0,
          name: '',
          country: '',
          playersQuantity: 11,
          enabled: true
        };

        this.loadTeams();
      },

      error: (err) => {
        console.error('Error al crear equipo:', err);
      }
    });
  }
  
  editTeam(team: Team): void {
    this.editingTeamId = team.id; 
    this.newTeam = {
      id: team.id,
      name: team.name,
      country: team.country,
      playersQuantity: team.playersQuantity,
      enabled: team.enabled
    };
    this.cdr.detectChanges();
  }

  updateTeam(): void {
    if (this.editingTeamId === null) { return;}

    console.log('Actualizando equipo:', this.newTeam);

    this.teamService.updateTeam(
      this.editingTeamId,
      this.newTeam
      ).subscribe({
        next: () => {
          console.log('Equipo actualizado correctamente');
          this.editingTeamId = null;
          this.newTeam = {
            id: 0,
            name: '',
            country: '',
            playersQuantity: 11,
            enabled: true
          };
          this.loadTeams();
        },
        error: (err) => {
          console.error('Error al actualizar equipo:', err);
        }
      });
  }

  cancelEdit(): void {
    this.editingTeamId = null;
    this.newTeam = {
      id: 0,
      name: '',
      country: '',
      playersQuantity: 11,
      enabled: true
    };
    this.cdr.detectChanges();
  }

}