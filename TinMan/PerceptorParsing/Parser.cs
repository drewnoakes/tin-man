using System.Collections.Generic;



using System;

namespace TinMan.PerceptorParsing {



internal sealed class Parser {
	public const int _EOF = 0;
	public const int _double = 1;
	public const int _ident = 2;
	public const int _message = 3;
	public const int maxT = 48;

    private const bool T = true;
    private const bool x = false;
    private const int minErrDist = 2;
    
    public Scanner scanner;
    public Errors  errors;

    public Token t;    // last recognized token
    public Token la;   // lookahead token
    private int errDist = minErrDist;

public string TeamName;
    
    private TimeSpan SimulationTime = System.TimeSpan.Zero;
    private TimeSpan GameTime = System.TimeSpan.Zero;
    private PlayMode PlayMode = PlayMode.Unknown;
    private FieldSide TeamSide = FieldSide.Unknown;
    private int? PlayerId;
    private double? AgentTemperature;
    private double? AgentBattery;
    private List<GyroState> GyroStates;
    private List<AccelerometerState> AccelerometerStates;
    private List<HingeState> HingeStates;
    private List<UniversalJointState> UniversalJointStates;
    private List<TouchState> TouchStates;
    private List<ForceState> ForceStates;
    private List<LandmarkPosition> LandmarkPositions;
    private List<PlayerPosition> TeamMatePositions;
    private List<PlayerPosition> OppositionPositions;
    private Polar? BallPosition;
    private List<HeardMessage> Messages;
    
    public PerceptorState State { get; private set; }

    private double AsDouble(string s) {
        double d;
        if (!double.TryParse(s, out d))
            SemErr("Unable to convert \"" + s + "\" to a double.");
        return d;
    }

    private void SeeLandmark(Polar pos, Landmark landmark) {
        if (LandmarkPositions==null)
            LandmarkPositions = new List<LandmarkPosition>(4);
        LandmarkPositions.Add(new LandmarkPosition(landmark, pos));
    }
    


    public Parser(Scanner scanner) {
        this.scanner = scanner;
        errors = new Errors();
    }

    private void SynErr (int n) {
        if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
        errDist = 0;
    }

    public void SemErr (string msg) {
        if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
        errDist = 0;
    }
    
    private void Get () {
        for (;;) {
            t = la;
            la = scanner.Scan();
            if (la.kind <= maxT) { ++errDist; break; }

            la = t;
        }
    }
    
    private void Expect (int n) {
        if (la.kind==n) Get(); else { SynErr(n); }
    }
    
    private bool StartOf (int s) {
        return set[s, la.kind];
    }
    
    private void ExpectWeak (int n, int follow) {
        if (la.kind == n) Get();
        else {
            SynErr(n);
            while (!StartOf(follow)) Get();
        }
    }

    private bool WeakSeparator(int n, int syFol, int repFol) {
        int kind = la.kind;
        if (kind == n) {Get(); return true;}
        else if (StartOf(repFol)) {return false;}
        else {
            SynErr(n);
            while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
                Get();
                kind = la.kind;
            }
            return StartOf(syFol);
        }
    }

	void Double(out double d) {
		Expect(1);
		d = AsDouble(t.val); 
	}

	void AngleInDegrees(out Angle a) {
		Expect(1);
		a = Angle.FromDegrees(AsDouble(t.val)); 
	}

	void Ident(out string s) {
		Expect(2);
		s = t.val; 
	}

	void TimeSpan(out TimeSpan time) {
		double secs; 
		Double(out secs);
		time = System.TimeSpan.FromSeconds(secs); 
	}

	void Vector3(out Vector3 v) {
		double x, y, z; 
		Double(out x);
		Double(out y);
		Double(out z);
		v = new Vector3(x, y, z); 
	}

	void Polar(out Polar pos) {
		double distance, angle1, angle2; 
		Double(out distance);
		Double(out angle1);
		Double(out angle2);
		pos = new Polar(distance, Angle.FromDegrees(angle1), Angle.FromDegrees(angle2)); 
	}

	void IntFlag(out bool isSet) {
		double d; 
		Double(out d);
		isSet = d != 0; 
	}

	void PolarPosExpr(out Polar pos) {
		Expect(4);
		Polar(out pos);
		Expect(5);
	}

	void MessageText(out string m) {
		Expect(2);
		m = t.val; 
	}

	void TimeExpr(out TimeSpan time) {
		Expect(6);
		Expect(7);
		Expect(8);
		TimeSpan(out time);
		Expect(5);
		Expect(5);
	}

	void GameStateExpr(out TimeSpan time, out PlayMode playMode, out int? playerId, out FieldSide? teamSide) {
		Expect(9);
		string pmStr; double playerIdDbl; playerId = null; teamSide = null; 
		if (la.kind == 10) {
			Get();
			Double(out playerIdDbl);
			Expect(5);
			playerId = (int)playerIdDbl; 
		}
		if (la.kind == 11) {
			Get();
			if (la.kind == 12 || la.kind == 13) {
				if (la.kind == 12) {
					Get();
					teamSide = FieldSide.Left; 
				} else {
					Get();
					teamSide = FieldSide.Right; 
				}
			}
			Expect(5);
		}
		Expect(7);
		Expect(14);
		TimeSpan(out time);
		Expect(5);
		Expect(7);
		Expect(15);
		Ident(out pmStr);
		Expect(5);
		if (!PlayModeUtil.TryParse(pmStr, out playMode))
		   SemErr("Unable to parse play mode '" + pmStr +"'.");;
		
		Expect(5);
	}

	void GyroStateExpr(out GyroState gyroState) {
		string label; double x,y,z; 
		Expect(16);
		Expect(7);
		Expect(17);
		Ident(out label);
		Expect(5);
		Expect(7);
		Expect(18);
		Double(out x);
		Double(out y);
		Double(out z);
		Expect(5);
		Expect(5);
		gyroState = new GyroState(label, x, y, z); 
	}

	void AccelerometerStateExpr(out AccelerometerState accState) {
		string label; Vector3 v; 
		Expect(19);
		Expect(7);
		Expect(17);
		Ident(out label);
		Expect(5);
		Expect(7);
		Expect(20);
		Vector3(out v);
		Expect(5);
		Expect(5);
		accState = new AccelerometerState(label, v); 
	}

	void HingeJointExpr(out HingeState hj) {
		string label; Angle angle; 
		Expect(21);
		Expect(7);
		Expect(17);
		Ident(out label);
		Expect(5);
		Expect(7);
		Expect(22);
		AngleInDegrees(out angle);
		Expect(5);
		Expect(5);
		hj = new HingeState(label, angle); 
	}

	void UniversalJointExpr(out UniversalJointState uj) {
		string label; Angle angle1, angle2; 
		Expect(23);
		Expect(7);
		Expect(17);
		Ident(out label);
		Expect(5);
		Expect(7);
		Expect(24);
		AngleInDegrees(out angle1);
		Expect(5);
		Expect(7);
		Expect(25);
		AngleInDegrees(out angle2);
		Expect(5);
		Expect(5);
		uj = new UniversalJointState(label, angle1, angle2); 
	}

	void TouchStateExpr(out TouchState ts) {
		string label; bool isTouching; 
		Expect(26);
		Expect(17);
		Ident(out label);
		Expect(27);
		IntFlag(out isTouching);
		Expect(5);
		ts = new TouchState(label, isTouching); 
	}

	void ForceStateExpr(out ForceState fs) {
		string label; Vector3 point, force; 
		Expect(28);
		Expect(7);
		Expect(17);
		Ident(out label);
		Expect(5);
		Expect(7);
		Expect(29);
		Vector3(out point);
		Expect(5);
		Expect(7);
		Expect(30);
		Vector3(out force);
		Expect(5);
		Expect(5);
		fs = new ForceState(label, point, force); 
	}

	void AgentStateExpr(out double temp, out double battery) {
		Expect(31);
		Expect(7);
		Expect(32);
		Double(out temp);
		Expect(5);
		Expect(7);
		Expect(33);
		Double(out battery);
		Expect(5);
		Expect(5);
	}

	void SeeExpr() {
		Expect(34);
		while (la.kind == 7) {
			Get();
			if (StartOf(1)) {
				VisibleItemExpr();
			} else if (la.kind == 44) {
				PlayerExpr();
			} else SynErr(49);
			Expect(5);
		}
		Expect(5);
	}

	void VisibleItemExpr() {
		string label = la.val; Polar pos; 
		switch (la.kind) {
		case 35: {
			Get();
			break;
		}
		case 36: {
			Get();
			break;
		}
		case 37: {
			Get();
			break;
		}
		case 38: {
			Get();
			break;
		}
		case 39: {
			Get();
			break;
		}
		case 40: {
			Get();
			break;
		}
		case 41: {
			Get();
			break;
		}
		case 42: {
			Get();
			break;
		}
		case 43: {
			Get();
			break;
		}
		default: SynErr(50); break;
		}
		PolarPosExpr(out pos);
		switch (label) {
		   case "F1L": SeeLandmark(pos, Landmark.FlagLeftTop); break;
		   case "F2L": SeeLandmark(pos, Landmark.FlagLeftBottom); break;
		   case "F1R": SeeLandmark(pos, Landmark.FlagRightTop); break;
		   case "F2R": SeeLandmark(pos, Landmark.FlagRightBottom); break;
		   case "G1L": SeeLandmark(pos, Landmark.GoalLeftTop); break;
		   case "G2L": SeeLandmark(pos, Landmark.GoalLeftBottom); break;
		   case "G1R": SeeLandmark(pos, Landmark.GoalRightTop); break;
		   case "G2R": SeeLandmark(pos, Landmark.GoalRightBottom); break;
		   case "B":   BallPosition = pos; break;
		   default: SemErr("Unable to parse visible item type string '"+label+"'."); return;
		}
		
	}

	void PlayerExpr() {
		string teamName; 
		double playerId;
		var parts = new List<BodyPartPosition>();
		
		Expect(44);
		Expect(11);
		Ident(out teamName);
		Expect(5);
		Expect(45);
		Double(out playerId);
		Expect(5);
		while (la.kind == 7) {
			string partLabel; Polar pos; 
			Get();
			Ident(out partLabel);
			PolarPosExpr(out pos);
			Expect(5);
			parts.Add(new BodyPartPosition(partLabel, pos)); 
		}
		bool isTeamMate = string.Equals(teamName, TeamName, StringComparison.Ordinal);
		var player = new PlayerPosition(isTeamMate, (int)playerId, parts);
		if (isTeamMate) {
		    if (TeamMatePositions==null) TeamMatePositions = new List<PlayerPosition>(4);
		    TeamMatePositions.Add(player);
		} else {
		    if (OppositionPositions==null) OppositionPositions = new List<PlayerPosition>(4);
		    OppositionPositions.Add(player);
		}
		
	}

	void HearExpr(out HeardMessage message) {
		TimeSpan time; Angle direction = Angle.NaN; string messageText; 
		Expect(46);
		TimeSpan(out time);
		if (la.kind == 47) {
			Get();
		} else if (la.kind == 1) {
			AngleInDegrees(out direction);
		} else SynErr(51);
		MessageText(out messageText);
		Expect(5);
		message = new HeardMessage(time, direction, new Message(messageText.Trim('\''))); 
	}

	void Perceptors() {
		while (StartOf(2)) {
			switch (la.kind) {
			case 6: {
				TimeSpan t; 
				TimeExpr(out t);
				SimulationTime = t; 
				break;
			}
			case 9: {
				TimeSpan t; PlayMode pm; int? id; FieldSide? side; 
				GameStateExpr(out t, out pm, 
out id, out side);
				GameTime = t; PlayMode = pm; PlayerId = id; 
				if (side.HasValue) TeamSide = side.Value; 
				break;
			}
			case 31: {
				double t, b; 
				AgentStateExpr(out t, out b);
				AgentTemperature = t; AgentBattery = b; 
				break;
			}
			case 16: {
				if (GyroStates==null) GyroStates = new List<GyroState>(1); GyroState gyroState; 
				GyroStateExpr(out gyroState);
				GyroStates.Add(gyroState); 
				break;
			}
			case 19: {
				if (AccelerometerStates==null) AccelerometerStates = new List<AccelerometerState>(1); AccelerometerState accState; 
				AccelerometerStateExpr(out accState);
				AccelerometerStates.Add(accState); 
				break;
			}
			case 21: {
				if (HingeStates==null) HingeStates = new List<HingeState>(1); HingeState hjState; 
				HingeJointExpr(out hjState);
				HingeStates.Add(hjState); 
				break;
			}
			case 23: {
				if (UniversalJointStates==null) UniversalJointStates = new List<UniversalJointState>(1); UniversalJointState ujState; 
				UniversalJointExpr(out ujState);
				UniversalJointStates.Add(ujState); 
				break;
			}
			case 26: {
				if (TouchStates==null) TouchStates = new List<TouchState>(1); TouchState tState; 
				TouchStateExpr(out tState);
				TouchStates.Add(tState); 
				break;
			}
			case 28: {
				if (ForceStates==null) ForceStates = new List<ForceState>(1); ForceState fState; 
				ForceStateExpr(out fState);
				ForceStates.Add(fState); 
				break;
			}
			case 34: {
				SeeExpr();
				break;
			}
			case 46: {
				if (Messages==null) Messages = new List<HeardMessage>(1); HeardMessage message; 
				HearExpr(out message);
				Messages.Add(message); 
				break;
			}
			}
		}
		State = new PerceptorState(
		                     SimulationTime, GameTime, PlayMode, TeamSide, PlayerId, 
		                     GyroStates, HingeStates, UniversalJointStates,
		                     TouchStates, ForceStates, AccelerometerStates,
		                     LandmarkPositions, TeamMatePositions, OppositionPositions, BallPosition,
		                     AgentBattery, AgentTemperature, Messages);
		
	}



    public void Parse() {
        la = new Token();
        la.val = "";        
        Get();
		Perceptors();

    Expect(0);
    }
    
    static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x},
		{x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, T,x,x,T, x,T,x,T, x,x,T,x, T,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x}

    };
} // end Parser

public struct ParseError {
    public int LineNumber { get; private set; }
    public int ColumnNumber { get; private set; }
    public int ErrorCode { get; private set; }
    public string Message { get; private set; }
    
    public ParseError(int line, int col, int code, string message) : this()
    {
        LineNumber = line;
        ColumnNumber = col;
        ErrorCode = code;
        Message = message;
    }

    public override string ToString() {
        var m = new System.Text.StringBuilder();
        m.Append(Message);
        bool hasInfo = LineNumber!=-1 || ColumnNumber!=-1 || ErrorCode!=-1;
        bool infoYet = false;
        if (hasInfo)
            m.Append(" (");
        if (LineNumber!=-1) {
            m.Append("line " + LineNumber);
            infoYet = true;
        }
        if (ColumnNumber!=-1) {
            if (infoYet) m.Append(", ");
            m.Append("col " + ColumnNumber);
            infoYet = true;
        }
        if (ErrorCode!=-1) {
            if (infoYet) m.Append(", ");
            m.Append("code " + ErrorCode);
        }
        if (hasInfo)
            m.Append(')');
        return m.ToString();
    }
}

public sealed class Errors {
    public List<ParseError> Items;
    public bool HasError { get { return Items!=null; } }
    public string ErrorMessages {
        get {
            if (Items==null) return null;
            var s = new System.Text.StringBuilder();
            foreach (var item in Items)
                s.AppendLine(item.ToString());
            return s.ToString();
        }
    }

    public void SynErr(int line, int col, int n) {
        string s;
        switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "double expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "message expected"; break;
			case 4: s = "\"(pol\" expected"; break;
			case 5: s = "\")\" expected"; break;
			case 6: s = "\"(time\" expected"; break;
			case 7: s = "\"(\" expected"; break;
			case 8: s = "\"now\" expected"; break;
			case 9: s = "\"(GS\" expected"; break;
			case 10: s = "\"(unum\" expected"; break;
			case 11: s = "\"(team\" expected"; break;
			case 12: s = "\"left\" expected"; break;
			case 13: s = "\"right\" expected"; break;
			case 14: s = "\"t\" expected"; break;
			case 15: s = "\"pm\" expected"; break;
			case 16: s = "\"(GYR\" expected"; break;
			case 17: s = "\"n\" expected"; break;
			case 18: s = "\"rt\" expected"; break;
			case 19: s = "\"(ACC\" expected"; break;
			case 20: s = "\"a\" expected"; break;
			case 21: s = "\"(HJ\" expected"; break;
			case 22: s = "\"ax\" expected"; break;
			case 23: s = "\"(UJ\" expected"; break;
			case 24: s = "\"ax1\" expected"; break;
			case 25: s = "\"ax2\" expected"; break;
			case 26: s = "\"(TCH\" expected"; break;
			case 27: s = "\"val\" expected"; break;
			case 28: s = "\"(FRP\" expected"; break;
			case 29: s = "\"c\" expected"; break;
			case 30: s = "\"f\" expected"; break;
			case 31: s = "\"(AgentState\" expected"; break;
			case 32: s = "\"temp\" expected"; break;
			case 33: s = "\"battery\" expected"; break;
			case 34: s = "\"(See\" expected"; break;
			case 35: s = "\"F1L\" expected"; break;
			case 36: s = "\"F2L\" expected"; break;
			case 37: s = "\"F1R\" expected"; break;
			case 38: s = "\"F2R\" expected"; break;
			case 39: s = "\"G1L\" expected"; break;
			case 40: s = "\"G2L\" expected"; break;
			case 41: s = "\"G1R\" expected"; break;
			case 42: s = "\"G2R\" expected"; break;
			case 43: s = "\"B\" expected"; break;
			case 44: s = "\"P\" expected"; break;
			case 45: s = "\"(id\" expected"; break;
			case 46: s = "\"(hear\" expected"; break;
			case 47: s = "\"self\" expected"; break;
			case 48: s = "??? expected"; break;
			case 49: s = "invalid SeeExpr"; break;
			case 50: s = "invalid VisibleItemExpr"; break;
			case 51: s = "invalid HearExpr"; break;

            default: s = "error code " + n; break;
        }
        
        AddError(line, col, n, s);
    }

    private void AddError(int line, int col, int code, string message) {
        if (Items==null)
            Items = new List<ParseError>();
        
        Items.Add(new ParseError(line, col, code, message));
    }

    public void SemErr(int line, int col, string s) {
        AddError(line, col, -1, s);
    }
    
    public void SemErr(string s) {
        AddError(-1, -1, -1, s);
    }
    
    public void Warning(int line, int col, string s) {
        // TODO should we record that this was a warning, and not an error?
        AddError(line, col, -1, s);
    }
    
    public void Warning(string s) {
        // TODO should we record that this was a warning, and not an error?
        AddError(-1, -1, -1, s);
    }
}


public sealed class FatalError: Exception {
    public FatalError(string m): base(m) {}
}

}