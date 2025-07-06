# Креатор на роботска рака - Вељко Аџиќ

Ова апликација овозможува на корисникот да изгради роботска рака од различни сегменти и додатоци, и да ја тестира во симулираната околина. Апликацијата содржи мени за изградба каде корисникот може да додава и брише сегменти, да ги поместува нивните позиции, и да управува со крајниот ефектор.

Во главниот формулар се извршува симулацијата каде раката го следи курсорот или движечко топче кое се одбива од рабовите. Корисникот дополнително може да го активира крајниот ефектор ако го кликне копчето на глувче.
Апликацијата исто така може да ги зачувува и вчитува креираните роботи во .rarm датотеки.

Овој проект е инспириран и базиран на ова [видео за инверзна кинематика](https://www.youtube.com/watch?v=RTc6i-7N3ms).

## Упатство

<img src="./imgs/mainForm.png" alt="Главниот формулар каде роботска рака го следи топче" title="Главен формулар со следење топче" style="width: 70%;">

<img src="./imgs/editor.png"  style="width: 70%;" alt="Формулар за едитирање каде корисникот во прегледот го активира ефекторот"><br>

Кога прв пат се вклучи апликацијата (прикажано на Сл.1) вчитан е стандардниот робот со 3 сегменти и пипало. Во горната лента има повеќе копчиња кои ни овозможуваат да креира нов робот, да се зачува во датотека, да се вчита од датотека, и да се едитира сегашниот робот.

Доколку курсорот е во прозорецот раката ќе го следи каде и да се движи. Ако корисникот го кликне левот копче на глувче крајниот ефектор ќе се активира. Кога курсорот го напушти прозорецот раката преминува да следи топче кое се движи низ прозорецот, одбивајќи се од рабовите.

### Сереализација

**Зачувување** на робот во датотека се извршува со кликање на едно од двете сини копчиња од лентата. Синото копче со пенкалото секогаш го зачувува роботот како нова датотека, а другото сино копче без пенкало го зачувува во веќе отворена датотека.
**Вчитување** на робот од датотека се врши со клик на копче со жолта папка каде од фајл менаџер се избира датотека со „.rarm“ екстензија.

### Креирање и менување на робот

Може да се отвори **мени за креација** (прикажано во Сл.2) кога се кликне плус копчето најлево или копчето со клуч најдесно на лентата. Во овој формула на левата страна има полиња за креирање на сегмент и менување на поставување краен ефектор; на десната страна има панел за преглед на раката.

**Додавање сегмент** на роботот се врши така што прво се поставува должината и бојата на сегментот, па се кликнува копчето „Додади сегмент“ каде се додава на врвот под ефекторот. **Поместување селектиран сегмент** се врши со копчињата со стрелки до листата.
**Бришење на сегмент** се врши на сличен начин каде што се селектира сегмент од листата и се кликне копчето „Избричи сегмент“.

Бојата на краен ефектор може да се постави со копчето **Ефектор боја**, а тип на ефекторот може да се одбере од опаѓачкото мени. Ефекторот може да биде од тип **пипало** (Grabber) или **ласер** (Laser).

## Решение на проблемот

Главните податоци и функционалности се чуваат во класите `Robot`, `Segment` и во главниот формулар `MainForm`.

### MainForm имплементација

Во `MainForm` се чува роботот кој се симулира, локација на датотека (ако не е отворена датотека се поставува на `null`, и знаменце за дали роботот бил изменет)

```csharp
public partial class MainForm : Form
{
    public Robot Rob { get; set; }
    public String FileLocation { get; set; }
    public bool Modified { get; set; }
}
```

Со овие функции се контролира однесувањето на роботот. Кога тајмерот изврши такт роботот го следи топчето, а кога глувчето се движи го следи курсорот.

```csharp
private void timer1_Tick(object sender, EventArgs e)
{
    Rob.Update();
    Invalidate();
}

private void MainForm_MouseMove(object sender, MouseEventArgs e)
{
    Rob.Update(e.X, e.Y);
    Invalidate();
}

private void MainForm_MouseLeave(object sender, EventArgs e)
{
    Rob.BallUpdate();
    timer1.Start();
}

private void MainForm_MouseEnter(object sender, EventArgs e)
{
    timer1.Stop();
}
```

Овие две функции вршат JSON сереализација на роботот. Користено е `JsonSerializer` наместо `BinaryFormatter` поради тоа што BinaryFormatter има грешка во неговата имплементација што се појави за сереализација на роботот. Грешката се појавуваше во десереализација на листа од сегменти каде од датотека се добива листа со точна големина, но со `null` елементи. Заради тоа се користи JsonSerializer.

```csharp
private void saveToFile()
{
    if(FileLocation == null)
    {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Robot Arm File (*.rarm)|*.rarm";
        sfd.Title = "Зачувај Робот";
        if(sfd.ShowDialog() == DialogResult.OK)
        {
            FileLocation = sfd.FileName;
        }
    }

    if( FileLocation != null)
    {
        using(FileStream fs = new FileStream(FileLocation, FileMode.OpenOrCreate, FileAccess.Write))
        {
            JsonSerializer.Serialize<Robot>(fs, this.Rob);
        }

        this.Modified = false;
    }
}

private void loadFromFile()
{
    if(FileLocation == null)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Robot Arm File (*.rarm)|*.rarm";
        ofd.Title = "Вчитај Робот";

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            FileLocation = ofd.FileName;
        }
    }

    if(FileLocation != null)
    {
        try
        {
            Robot newRob;
            using (FileStream fs = new FileStream(FileLocation, FileMode.Open, FileAccess.Read))
            {
                newRob = JsonSerializer.Deserialize<Robot>(fs);
            }
            this.Rob.BuildFrom(newRob.Segments, newRob.EndEffector);
            this.Modified = false;
        } catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }
}
```

### Robot имплементација

Во класа `Robot` се чуваат овие параметри. Сите параметри означени со `[JsonIgnore]` не се сереализираат. Параметарот `Base` го има `[JsonConverter(typeof(JsonVec2Converter))]` бидејќи `Vector2` класата нема имплементација за сереализација во JSON. Класата `DummyBall` ја содржи имплементацијата на топчето кое роботот го следи.

```csharp
public class Robot
{
    public List<Segment> Segments { get; set; }
    public int Length { get; set; }
    [JsonConverter(typeof(JsonVec2Converter))]
    public Vector2 Base { get; set; }
    [JsonIgnore]
    public bool FollowMouse { get; set; } = false;
    [JsonIgnore]
    public DummyBall Ball { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    [JsonIgnore]
    public bool PreviewMode { get; set; } = false;
    public Effector EndEffector { get; set; }
    [JsonIgnore]
    public List<Segment> AllSegments { get; set; }
}
```

Во функцијата `AddSegment()` се додава нов сегмент во листата `Segments` и се ажурира `EndEffector` да е поврзан на новиот сегмент, се ажурира топчето со `this.BallUpdate()` каде ја поставува позицијата на топчето да биде на врвот на ефекторот, и се поставува листата од сите сегменти со крајниот ефектор `AllSegments`.

```csharp
public void AddSegment(double len, Color? c = null)
{
    if (Length == 0)
        Segments.Add(new Segment(Base, len, 0, c)); // Dodavanje na prv segment
    else
    {   // Dodavanje na sleden segment
        Segment last = Segments.Last();
        Segments.Add(
            new Segment(last, len, c)
        );
    }

    EndEffector.Rebase(Segments.Last().end);
    this.BallUpdate();

    this.AllSegments = new List<Segment>(this.Segments);
    this.AllSegments.Add(this.EndEffector);

    Length++;
}
```

Со функцијата `Update()` роботот следи некој цел. Ако не се даде аргумент роботот ќе го следи топчето, а ако се дадени координати (X, Y) кои се во границите на прозорецот тогаш ја следи таа позиција.

```csharp
public void Update()
{   // Update so sledenje na topche
    this.Update(-100, -100);
}
public void Update(float X, float Y)
{
    this.FollowMouse = X > 0 && X < Width && Y > 0 && Y < Height;
    if (FollowMouse) BallUpdate();

    // cel za sledenje
    Vector2 target = FollowMouse || PreviewMode ? new Vector2(X, Y) : Ball.Pos();
    Segment s;
    for (int i = Length; i >= 0; i--)
    {
        s = AllSegments[i];
        s.target(target.X, target.Y);
        target = s.pos;  // da bidat povrzani segmenti
    }

    // Da ne se pomestuvaat od bazata
    Vector2 b = Base;
    foreach (Segment seg in AllSegments)
    {
        seg.Rebase(b);
        b = seg.end;
    }

    Ball.Update(Width, Height);

}
```

Функцијата `BuildFrom()` се користи за изгради робот од дадена листа сегменти и од краен ефектор. Има додатен аргумент `Rebase` која определува дали роботот да ја користи својата `Base` променлива да ги врати сегментите или да ја помести својата `Base` каде што се сегментите. Функцијата се користи при праќање на робот променливи помеѓу **менито за градба** и изградување од десереализација.

```c#
public void BuildFrom(List<Segment> segs, Effector ef, bool Rebase = false)
{   // Recreacija na robot of dadeni segmenti i efektor
    if (!Rebase) // Bez promena na pozicija na segmenti
    {
        this.Segments = segs;
        this.Length = this.Segments.Count;
        this.Base = new Vector2(segs[0].pos.X, segs[0].pos.Y);
    }
    else
    {   // So promena na pozicija do Base
        this.Segments.Clear();
        this.Length = 0;
        foreach (var segment in segs)
        {
            this.AddSegment(segment.len, segment.Colour);//, segment.range_min, segment.range_max);
        }
    }

    this.EndEffector = new Effector(ef);
    this.EndEffector.Rebase(this.Segments.Last().end);
    this.AllSegments = new List<Segment>(this.Segments);
    this.AllSegments.Add(this.EndEffector);

    this.BallUpdate();
}
```

### Segment имплементација

Класата `Segment` ги содржи овие параметри. Параметрите `pos` и `end` се вектори затоа го имаат флегот `[JsonConverter(typeof(JsonVec2Converter))]`, а `Colour` го има флегот `[JsonConverter(typeof(JsonColorConverter))]` бидејќи, како и вектор класата, исто така не е има имплементација на JSON сереализација.

```c#
public class Segment
{
    [JsonConverter(typeof(JsonVec2Converter))]
    public Vector2 pos { get; set; }
    [JsonConverter(typeof(JsonVec2Converter))]
    public Vector2 end { get; set; }
    public double angle { get; set; }
    public double len { get; set; }
    public double change { get; set; }
    public static double OFFSET = (-90 * Math.PI / 180);
    [JsonIgnore]
    public static Color DEFAULT_COLOUR = Color.FromArgb(205, 140, 25);
    [JsonConverter(typeof(JsonColorConverter))]
    public Color Colour { get; set; }
}
```

Во `calculateEnd()` се пресметува вредноста на `end` променливата за да должината е точна. А методот `target()` го пресметува аголот на промена `change` и го поставува сегментот да покажува до целта.

```C#
protected void calculateEnd()
{
    double a = angle + change; // vistinski agol
    // presmetka na nov end
    float nX = pos.X + (float)(len * Math.Cos(a));
    float nY = pos.Y + (float)(len * Math.Sin(a));
    end = new Vector2(nX, nY);
}

public void target(float X, float Y)
{   // Funkcija za sledenje na cel
    // Presmetka na agol od baza na segment do target
    Vector2 t = new Vector2(X, Y);
    Vector2 dir = t - pos;
    // Presmetka na noviot agol
    change = Math.Atan2(dir.Y, dir.X) - OFFSET;

    // Postavuvanje velichina so obratna nasoka na dir vektor
    dir = Vector2.Normalize(dir);
    dir = Vector2.Multiply(dir, (float)(-len));

    // pomestuvanje baza da pokazuva segemnt to target
    pos = t + dir;

    // presmetka na end
    calculateEnd();
}
```

Ова е помошна функција која го поместува сегментот на дадена позиција `b` и го пресметува крајот.

```C#
public void Rebase(Vector2 b)
{   // Pomoshna funkcija za vrakjanje na lokacija b
    this.pos = b;
    calculateEnd();
}
```

### Effector имплементација

Класата `Effector` наследува од `Segment` и во неа има само дполнителни параметри `Active` (флег за активност) и `Type` (типот на ефектор). Исто така во тој `namespace` го има и `enum EffectorType`.

```C#
public enum EffectorType
{
    Grabber,
    Laser
};

public class Effector : Segment
{
    public bool Active { get; set; }
    public EffectorType Type { get; set; }
}
```

Класата содржи празен default конструктор, конструктор кој креира ефектор на одредена позиција и од одреден тип, и сopy конструктор кој копира параметри од некој друк ефектор.

```C#
public Effector() : base() { }

public Effector(Vector2 pos, EffectorType Type = EffectorType.Grabber)
:base(pos, 50.0, 0)
{
    this.Type = Type;
    this.Active = false;
}

// Copy konstruktor
public Effector(Effector ot)
:base(ot, ot.len, ot.Colour)
{
    this.Type = ot.Type;
    this.Active = ot.Active;
}
```

## Користени ресурси

За овој проект се користени икони од [Google Fonts](https://fonts.google.com/icons) за копчињата од статусната линија, и икона од [Noun Project](https://thenounproject.com/icon/robotic-arm-7847123/) за слика на прозорците во горниот лев ќош.

### Вештачка интелигенција

Во текот на ова проектна е користен GPT-4o модел за генерално дебагирање, и наоѓање информации за докумнетација.

Ова порака веше користена да се додаде `Assets` фолдер при компајлација да се вклучени икони и стандарден робот.

> I have a .net wfa that has classes that are serialised. I want to load a default example when the program starts.
> The file that i want to load is located in ProjectName/ProjectName/Assets/DefaultFile.rarm.
> In my code in the constructor of the form I should set the FileLocation prop to the absolute path to the file and then call the loadFromFile() method (it uses the prop). How can I do this?

Ова Порака е користена да се најде точно како да се имплементира JSON сереализација и десереализација на `Vector2` класата.

> Forget all context. How do I overload the Read method in JsonVec2Converter?
> ```C#
> public class JsonVec2
> {
> public float? X { get; set; }
> public float? Y { get; set; }
> }
> public class JsonVec2Converter : JsonConverter<Vector2>
> {
> public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
> {
> // TODO
> }
> public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
> {
> var vec = new JsonVec2
> {
> X = value.X,
> Y = value.Y
> };
> var res = JsonSerializer.Serialize(vec);
> writer.WriteStringValue(res);
> }
> }
> ```
