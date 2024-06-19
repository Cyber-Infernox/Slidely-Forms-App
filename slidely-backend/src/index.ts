import express, { Request, Response } from "express";
import bodyParser from "body-parser";
import fs from "fs";
import path from "path";

const app = express();
const PORT = 3000;
const dbFilePath = path.join(__dirname, "db.json");

app.use(bodyParser.json());

app.get("/ping", (req: Request, res: Response) => {
  res.json(true);
});

app.post("/submit", (req: Request, res: Response) => {
  const { name, email, phone, github_link, stopwatch_time } = req.body;
  const newEntry = { name, email, phone, github_link, stopwatch_time };

  let db = [];
  if (fs.existsSync(dbFilePath)) {
    db = JSON.parse(fs.readFileSync(dbFilePath, "utf-8"));
  }

  db.push(newEntry);
  fs.writeFileSync(dbFilePath, JSON.stringify(db, null, 2));

  res.json({ success: true });
});

// New endpoint to fetch all submissions
app.get("/read", (req: Request, res: Response) => {
  let db = [];

  if (fs.existsSync(dbFilePath)) {
    db = JSON.parse(fs.readFileSync(dbFilePath, "utf-8"));
  }

  res.json(db);
});

app.delete("/submissions", (req: Request, res: Response) => {
  const { email } = req.body;
  let db = [];
  if (fs.existsSync(dbFilePath)) {
    db = JSON.parse(fs.readFileSync(dbFilePath, "utf-8"));
  }

  const index = db.findIndex((submission: any) => submission.email === email);
  if (index >= 0) {
    db.splice(index, 1);
    fs.writeFileSync(dbFilePath, JSON.stringify(db, null, 2));
    res.json({ success: true });
  } else {
    res.status(404).json({ error: "Submission not found" });
  }
});

interface Submission {
  name: string;
  email: string;
  phone: string;
  github_link: string;
  stopwatch_time: string;
}

app.put("/update", (req: Request, res: Response) => {
  const email: string = req.query.email as string;
  const { name, phone, github_link, stopwatch_time }: Submission = req.body;

  if (!email) {
    return res.status(400).json({ error: "Email is required" });
  }

  let db: Submission[] = [];
  if (fs.existsSync(dbFilePath)) {
    db = JSON.parse(fs.readFileSync(dbFilePath, "utf-8"));
  }

  const index = db.findIndex((entry: Submission) => entry.email === email);

  if (index !== -1) {
    db[index] = { name, email, phone, github_link, stopwatch_time };
    fs.writeFileSync(dbFilePath, JSON.stringify(db, null, 2));
    res.json({ success: true });
  } else {
    res.status(404).json({ error: "Submission not found" });
  }
});

app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});
