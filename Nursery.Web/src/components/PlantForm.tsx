import { Options } from "../types/Options"

interface PlantFormProps {
  form: {
    name: string
    type: string
    lifeCycle: string
    floweringStatus: boolean
  }
  options: Options
  saving: boolean
  error: string | null
  onChange: (field: string, value: string | boolean) => void
  onSubmit: () => void
  onCancel: () => void
  submitLabel: string
}

function PlantForm({ form, options, saving, error, onChange, onSubmit, onCancel, submitLabel }: PlantFormProps) {
  return (
    <div className="detail-card">
      <div className="detail-header">
        <h1>New Plant</h1>
      </div>
      {error && <p className="form-error">{error}</p>}
      <div className="detail-rows">
        <div className="detail-row">
          <span className="label">Name</span>
          <input
            className="detail-input"
            type="text"
            placeholder="Plant name"
            value={form.name}
            onChange={e => onChange("name", e.target.value)}
          />
        </div>
        <div className="detail-row">
          <span className="label">Type</span>
          <select
            className="detail-select"
            value={form.type}
            onChange={e => onChange("type", e.target.value)}
          >
            {options.plantTypes.map(t => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
        </div>
        <div className="detail-row">
          <span className="label">Life Cycle</span>
          <select
            className="detail-select"
            value={form.lifeCycle}
            onChange={e => onChange("lifeCycle", e.target.value)}
          >
            {options.lifeCycleTypes.map(l => (
              <option key={l} value={l}>{l}</option>
            ))}
          </select>
        </div>
        <div className="detail-row">
          <span className="label">Flowering</span>
          <select
            className="detail-select"
            value={form.floweringStatus ? "Yes" : "No"}
            onChange={e => onChange("floweringStatus", e.target.value === "Yes")}
          >
            <option value="Yes">Yes</option>
            <option value="No">No</option>
          </select>
        </div>
      </div>
      <div className="edit-actions">
        <button className="save-button" onClick={onSubmit} disabled={saving}>
          {saving ? "Saving..." : submitLabel}
        </button>
        <button className="cancel-button" onClick={onCancel} disabled={saving}>
          Cancel
        </button>
      </div>
    </div>
  )
}

export default PlantForm