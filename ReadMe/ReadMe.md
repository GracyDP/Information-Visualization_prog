<p align="center">
  <h2 align="center">Guida autonoma per una smart-city</h2>
</p>

<p align="center">  
  <img src=".png" width="50%">  
</p>
<p align="center">
  <sub> Figura1: La seguente immagine mostra l'ambiente in cui si è svolto il progetto.  </sub>  
</p>


## 📌 Descrizione del Progetto
Questo progetto utilizza **Unity-Hub** e **ML-Agents** per allenare un agente a percorrere una normale strada cittadina fino ad arrivare al suo obiettivo finale: **parcheggiare**.
Suddivisa in varie zone che, comprendono incroci con semafori e segnaletica, rotonde e numerose curve; la sfida di questo progetto è volta a insegnare all'agente come cimentarsi nell'attraversare correttamente tutti questi ostacoli nella piccola sottoarea di una città. 
<p align="center">  
  <img src=".png" width="80%">  
</p>
<p align="center">
  <sub>Figura 2: Immagine esplicativa della suddivisione delle sottoaree di cui è composto il progetto. </sub>
</p>

## 📖 Indice:
- [Obiettivi](#obiettivi)
- [Dati di Input](#dati-di-input)
- [Strumenti e Tecnologie Utilizzati](#strumenti-e-tecnologie-utilizzati)
- [Processo e Metodologia](#processo-e-metodologia)
- [Istruzioni per l'Uso](#istruzioni-per-luso)
- [Contatti e Riferimenti](#contatti-e-riferimenti)

---

## Obiettivi:
Il progetto si propone di:  
✔️ Percorrere l'intera smart-city riuscendo a gestire correttaente il proprio senso di marcia.  
✔️ Riuscire a superare correttamente ostacoli come: rotonde, curve, semafori, segnali di stop e un passante (identificato nel seguente progetto da un gatto).  
✔️ Parcheggiare in uno dei parcheggi disponibili, identificandolo tramite un `semaforo`.   

---

## 📊 Dati di Input:
- **Una scena in Unity** (`ScenaMadre.unitypackage`)  
<sub>Ottenuta sfruttando vari assets disponibili gratuitamente sull'assets store di Unity3D</sub>

## 📂Dati di Output:
- **Scena in Unity** (`ScenaMadre.unitypackage`)
- **Folder results** (`Dati relativi ai risultati sull'allenamento dell'agente`)
- **Cartella TensorBoard** (`Dati e png relativi ai risultati degli allenamnti`)
-**File ppo** (`File yaml sul contenuto dell'addestramento`)

---

## 🛠 Strumenti e Tecnologie Utilizzati
- **Unity Hub** → Per la creazione della scena e l'ambiente di allenamento.
- **Conda** → Terminale utilizzato per inizializzare ed elaborare l'allenamento.
- **C#** → Linguaggio in cui vengono programmati i singoli script.

---

## 🧩 Processo e Metodologia  
Per il seguente progetto durante la fase iniziale è stato necessario configurare l'ambiente, successivamente è stata creata la scenaMadre in cui si svolge il nostro allenamento.
Si sono utilizzate due metodologie differenti per eseguire l'allenamento dell'agente:

### 1️⃣ **Metodo wall**
Sono stati applicati lungo tutta l'area perimetrale al marciapiede e per distinguere i due sensi di marcia dei `GameObject muro` gestiti con metodo OnTrigger che indica la collisione con l'oggetto che ha `Reward negativo`.  
 Attraverso i reward l'agente riesce a capire quali azioni può svolgere e quali azioni deve completamente evitare.

<p align="center" ><img src=.png> </p>
<p align="center">
  <sub> Figura 3: La seguente immagine mostra come sono stati posizionati i wall all'interno della scena.  </sub> </p>

### 2️⃣ **Metodo recorder**
Per sfruttare al massimo le capacità dell'agente, è stato inserito anche un file di "ricordo" che permette di avere memoria di un determinato comportamento (questo file rappresenta un incrocio tra i dati relativi all'ambiente che circonda l'agente e il tipo di azione che gli viene chiesto di svolgere).  Per evitare che l'agente fosse troppo dipendente da questo tipo di comportamento è stata inserita una low`strength` (dipendenza).


### 3️⃣ **Creazione di semafori**
Nella seguente scena sono stati utilizzati due tipi di semafori:  
*Con TrafficLights* -> Intendiamo un classico semaforo che dipendentemente dal tipo di luce(Rossa,Verde,Gialla) e chiarisce se la macchina può attraversare, deve fermarsi o rallentare.  
*Con ParckingLights* -> Definiamo se il parcheggio è pieno o meno, in questo caso il semaforo è composto da solo due luci(Rossa,Verde) la prima si accende nel caso in cui non ci siano posti nel parcheggio designato, l'altra fa invece capire all'agente che è possibile parcheggiare e che quindi si può entrare in quell'area.

<p align="center"><img src=.png></p>
<p align="center">
  <sub> Figura 4: Immagine di un agente in prossimità di un semaforo attivo.  </sub> </p>

**📌NOTA:** I seguenti semafori possegono una Light di tipo `spotLight` con materiale del rispettivo colore identificativo.  

### 4️⃣ **Gestione Incroci**


### **Gestione dell' entita passante**


### **Gestione backgroud sonoro***





***********************************Definire le librerie usate
```markdown

```
Utilizzando **GameObject invisibili** è stato possibile gestire gli incroci in maniera sistematica dipendentemente dal lato in cui si trova l'agente.

```markdown

```

```markdown

```

Infine
```markdown

```

📌 **Nota**: .

<p align="center"> <img src=immagine5.png> </p>
<p align="center"><sub> Figura 5: La seguente tabella rappresenta i dati appena citati.</sub></p>

### 5️⃣ **Aggregazione per Categoria** 

⚙️ 

```sql

```
<p align="center"><sub> . </sub>  </p>


📌 **Risultato**: .

<p align="center"> <img src=.png> </p>
<p align="center">
  <sub>Figura 6::
  <ul>
    <li>La .</li>
    <li>La .</li>
    <li>Il .</li>
  </ul>
  </sub>
</p>

<sub> </sub>


### 6️⃣ **Analisi Risultati**
A seguito della ...
✔️    
✔️   
✔️   
```markdown

```
<sub>    </sub>  
   
```markdown

```
<sub>    </sub>    

<sub>   </sub>    

<p align="center"> <img src=immagine8.png> </p>
<p align="center"><sub> Figura 5: .</sub></p>  


_**Ora è possibile **_  

<sub>Un possibile esempio:</sub>  
<p align="center"><sub> .</sub></p>  caricare video dell'agente
<p align="center"> <img src=.png> </p>
<p align="center"><sub> Figura 9: Esempio di comprensione dei dati.</sub></p>    

---

## 📌 Istruzioni per l'Uso

1️⃣ **Aprire** il progetto in Unity Hub.  
2️⃣ **Caricare** i file .  
3️⃣ **Utilizzare** il .  

---

## 📑🖼️ ULTERIORI IMMAGINI esplicative


---

## 📩 Contatti e Riferimenti

- **Autori**: Di Pietro Grazia e Giuliano Luca.
- **Email**: g.dipietro4@studenti.unisa.it e l.giuliano12@studenti.unisa.it

---

