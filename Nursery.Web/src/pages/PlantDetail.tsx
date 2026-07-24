import { useState, useEffect } from "react"
import { useParams, useNavigate } from "react-router-dom"
import { Plant } from "../types/Plant"
import { Options } from "../types/Options"
import "../App.css"

function PlantDetail() {
    const { plantId } = useParams<{ plantId: string }>()
    const navigate = useNavigate()

    const [plant, setPlant] = useState<Plant | null>(null)
    const [options, setOptions] = useState<Options | null>(null)
    const [loading, setLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)
    const [editMode, setEditMode] = useState<boolean>(false)
    const [edited, setEdited] = useState<Plant | null>(null)
    const [saving, setSaving] = useState<boolean>(false)

    useEffect (() => {
        Promise.all([
            fetch(`https://localhost:7288/api/plants/id/${plantId}`).then(res => {
                if (!res.ok) throw new Error("Plant not found")
                    return res.json()
            }),
            fetch("https://localhost:7288/api/options").then(res => {
                if (!res.ok) throw new Error("Failed to load options")
                    return res.json()
            })

        ])
        .then(([plantData, optionsData]: [Plant, Options]) => {
            setPlant(plantData)
            setEdited(plantData)
            setOptions(optionsData)
            setLoading(false)
        })
        .catch((err: Error) => {
            setError(err.message)
            setLoading(false)
        })
    }, [plantId])

    const handleSave = () => {
        if (!edited) return
        setSaving(true)

        fetch(`https://localhost:7288/api/plants/id/${plantId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(edited)
        })
        .then(res => {
            if (!res.ok) throw new Error("Failed to save changes")
                return res.json()
        })
        .then((updated: Plant) => {
            setPlant(updated)
            setEditMode(false)
            setSaving(false)
        })
        .catch((err: Error) => {
            setError(err.message)
            setSaving(false)
        })
    }
    
    const handleCancel = () => {
        setEdited(plant)
        setEditMode(false)
    }

    if (loading) return <div className="status">Loading plant...</div>
    if (error) return <div className="status error">Error: {error}</div>
    if (!plant || !edited || !options) return <div className="status">Plant not found.</div>

  return (
    <div className="app">
      <button className="back-button" onClick={() => navigate("/")}>
        ← Back
      </button>
      <div className="detail-card">
        <div className="detail-header">
          <h1>{plant.name}</h1>
          {!editMode && (
            <button className="edit-button" onClick={() => setEditMode(true)}>
              Edit
            </button>
          )}
        </div>
        <div className="detail-rows">
          <div className="detail-row">
            <span className="label">Type</span>
            {editMode ? (
              <select
                className="detail-select"
                value={edited.type}
                onChange={e => setEdited({ ...edited, type: e.target.value })}
              >
                {options.plantTypes.map(t => (
                  <option key={t} value={t}>{t}</option>
                ))}
              </select>
            ) : (
              <span className="value">{plant.type}</span>
            )}
          </div>
          <div className="detail-row">
            <span className="label">Life Cycle</span>
            {editMode ? (
              <select
                className="detail-select"
                value={edited.lifeCycle}
                onChange={e => setEdited({ ...edited, lifeCycle: e.target.value })}
              >
                {options.lifeCycleTypes.map(l => (
                  <option key={l} value={l}>{l}</option>
                ))}
              </select>
            ) : (
              <span className="value">{plant.lifeCycle}</span>
            )}
          </div>
          <div className="detail-row">
            <span className="label">Flowering</span>
            {editMode ? (
              <select
                className="detail-select"
                value={edited.floweringStatus ? "Yes" : "No"}
                onChange={e => setEdited({ ...edited, floweringStatus: e.target.value === "Yes" })}
              >
                <option value="Yes">Yes</option>
                <option value="No">No</option>
              </select>
            ) : (
              <span className="value">{plant.floweringStatus ? "Yes" : "No"}</span>
            )}
          </div>
        </div>
        {editMode && (
          <div className="edit-actions">
            <button className="save-button" onClick={handleSave} disabled={saving}>
              {saving ? "Saving..." : "Save"}
            </button>
            <button className="cancel-button" onClick={handleCancel} disabled={saving}>
              Cancel
            </button>
          </div>
        )}
      </div>
    </div>
  )
}

export default PlantDetail